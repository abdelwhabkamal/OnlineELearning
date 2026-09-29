using Microsoft.EntityFrameworkCore;
using OELearning.Api.Models.Entities;
using OELearning.Api.Models.Enums;

namespace OELearning.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Centers.AnyAsync()) return;

        // 1. Centers (Matching screenshot 2)
        var centerLouran = new Center { Name = "Louran Academy - S2", Address = "Louran, Alexandria", ContactPhone = "0100000001", IsOnline = false };
        var centerAlpha = new Center { Name = "Alpha Station - S2", Address = "Alpha Station, Alexandria", ContactPhone = "0100000002", IsOnline = false };
        var centerSmouha = new Center { Name = "Smouha Academy - S2", Address = "Smouha, Alexandria", ContactPhone = "0100000003", IsOnline = false };
        var centerSediBeshr = new Center { Name = "Sedi beshr - S2", Address = "Sidi Bishr, Alexandria", ContactPhone = "0100000004", IsOnline = false };
        var centerOnline = new Center { Name = "Online - S2", Address = "Virtual Platform", ContactPhone = "0100000005", IsOnline = true };

        context.Centers.AddRange(centerLouran, centerAlpha, centerSmouha, centerSediBeshr, centerOnline);
        await context.SaveChangesAsync();

        // 2. Academic Year
        var academicYear = new AcademicYear { Name = "Secondary 3 - 3rd Sec" };
        context.AcademicYears.Add(academicYear);
        await context.SaveChangesAsync();

        // 3. Groups (Matching screenshot 1)
        var grpOnline = new Group { Name = "Online - Group O", CenterId = centerOnline.Id, AcademicYearId = academicYear.Id };
        var grpLouran = new Group { Name = "Louran - Group L", CenterId = centerLouran.Id, AcademicYearId = academicYear.Id };
        var grpAlpha = new Group { Name = "Alpha Station - Group F", CenterId = centerAlpha.Id, AcademicYearId = academicYear.Id };

        context.Groups.AddRange(grpOnline, grpLouran, grpAlpha);
        await context.SaveChangesAsync();

        // 4. Center Schedules (Lecture sessions in center)
        context.CenterSchedules.AddRange(
            new CenterSchedule { CenterId = centerLouran.Id, LectureTitle = "Physics - Session 1", SessionTime = new DateTime(2026, 9, 3, 14, 0, 0) },
            new CenterSchedule { CenterId = centerAlpha.Id, LectureTitle = "Physics - Session 1", SessionTime = new DateTime(2026, 9, 3, 16, 30, 0) },
            new CenterSchedule { CenterId = centerSmouha.Id, LectureTitle = "Physics - Session 1", SessionTime = new DateTime(2026, 9, 3, 18, 0, 0) },
            new CenterSchedule { CenterId = centerSediBeshr.Id, LectureTitle = "Physics - Session 1", SessionTime = new DateTime(2026, 9, 3, 12, 0, 0) },
            new CenterSchedule { CenterId = centerOnline.Id, LectureTitle = "Physics - Session 1", SessionTime = new DateTime(2026, 9, 3, 20, 0, 0) }
        );

        // 5. Admin & Teacher
        var adminPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123456");
        var admin = new User
        {
            FullName = "OELearning System Admin",
            PhoneNumber = "01000000000",
            Email = "admin@OELearning.com",
            PasswordHash = adminPassword,
            Role = UserRole.Admin
        };
        context.Users.Add(admin);

        // 6. Demo Students (Exact records seen in screenshot 1)
        var studentData = new[]
        {
            ("Jana Ahmed And El Moniem El barbary", "01099277423", "01062047172", "641FF", grpOnline.Id, StudentStatus.Approved),
            ("???? ???? ??? ????", "01091743748", "01228054645", "076QT", grpOnline.Id, StudentStatus.Approved),
            ("Zamzam Waleed elsayed", "01008290343", "01205160898", "977UI", grpLouran.Id, StudentStatus.Approved),
            ("Yara Paul liliy", "01555469633", "01102566776", "614TZ", grpAlpha.Id, StudentStatus.Approved),
            ("Yassin Assistant", "01274966249", "0106142279", "646WU", grpLouran.Id, StudentStatus.Approved)
        };

        var studentPwd = BCrypt.Net.BCrypt.HashPassword("Student@123");
        foreach (var (name, phone, parentPhone, code, grpId, status) in studentData)
        {
            var user = new User
            {
                FullName = name,
                PhoneNumber = phone,
                Email = $"{code.ToLower()}@student.OELearning.com",
                PasswordHash = studentPwd,
                Role = UserRole.Student
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var profile = new StudentProfile
            {
                UserId = user.Id,
                OELearningCode = code,
                ParentPhoneNumber = parentPhone,
                GroupId = grpId,
                Status = status,
                QrCodePayload = $"OELearning-STU-{code}-{phone}"
            };
            context.StudentProfiles.Add(profile);
        }

        // 7. Course & Curriculum
        var course = new Course
        {
            Title = "General Physics 2026 / 2027",
            Description = "Comprehensive curriculum for Secondary 3",
            AcademicYearId = academicYear.Id
        };
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var unit1 = new CurriculumUnit
        {
            CourseId = course.Id,
            Title = "?????? ??????: ?????? ??????? ?????? ??? ??????? ??????",
            Order = 1
        };
        context.CurriculumUnits.Add(unit1);
        await context.SaveChangesAsync();

        var lecture1 = new Lecture
        {
            UnitId = unit1.Id,
            Title = "???????? 1: ?????? ??????? ???? ????? ????????? ???????",
            Description = "??? ?????? ?? ?? ????? ???? ??????? ???? ???????",
            VideoUrl = "https://stream.OELearning.com/videos/phys-unit1-lec1.mp4",
            AttachmentPdfUrl = "https://files.OELearning.com/pdfs/unit1-notes.pdf",
            Price = 100,
            IsFree = false,
            Order = 1
        };
        context.Lectures.Add(lecture1);
        await context.SaveChangesAsync();

        // 8. Exam Sample
        var exam1 = new Exam
        {
            LectureId = lecture1.Id,
            Title = "?????? ????? ???????? ??????",
            DurationMinutes = 30,
            TotalMarks = 20,
            PassingMarks = 10
        };
        context.Exams.Add(exam1);
        await context.SaveChangesAsync();

        var q1 = new Question
        {
            ExamId = exam1.Id,
            QuestionText = "??? ????? ??? ??? ???? ??? ????? ?????? ????? ????? ??? ?????? ??? ??????? ????????:",
            Marks = 5
        };
        context.Questions.Add(q1);
        await context.SaveChangesAsync();

        context.QuestionOptions.AddRange(
            new QuestionOption { QuestionId = q1.Id, OptionText = "????? ??? 4 ???????", IsCorrect = true },
            new QuestionOption { QuestionId = q1.Id, OptionText = "??? ?????", IsCorrect = false },
            new QuestionOption { QuestionId = q1.Id, OptionText = "??? ??? ?????", IsCorrect = false },
            new QuestionOption { QuestionId = q1.Id, OptionText = "????? ????? ???", IsCorrect = false }
        );

        await context.SaveChangesAsync();
    }
}
