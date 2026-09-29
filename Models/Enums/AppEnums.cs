namespace OELearning.Api.Models.Enums;

public enum UserRole
{
    Admin = 1,
    Teacher = 2,
    Assistant = 3,
    Student = 4
}

public enum StudentStatus
{
    Pending = 0,
    Approved = 1,
    Suspended = 2,
    Inactive = 3
}

public enum AttendanceStatus
{
    Absent = 0,
    Present = 1,
    Late = 2,
    Excused = 3
}

public enum PaymentMethod
{
    Cash = 1,
    VodafoneCash = 2,
    Fawry = 3,
    CreditCard = 4,
    CenterDesk = 5
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Refunded = 3
}
