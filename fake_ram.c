#include <stdio.h>
#include <sys/sysinfo.h>

int sysinfo(struct sysinfo *info) {
    static int (*real_sysinfo)(struct sysinfo *) = NULL;
    if (!real_sysinfo) {
        real_sysinfo = __builtin_return_address(0);
    }
    // Return 4GB total RAM so SQL Server allows startup on 1GB instances
    info->totalram = 4ULL * 1024 * 1024 * 1024;
    info->freeram  = 2ULL * 1024 * 1024 * 1024;
    return 0;
}
