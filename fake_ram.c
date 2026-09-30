#define _GNU_SOURCE
#include <dlfcn.h>
#include <sys/sysinfo.h>

int sysinfo(struct sysinfo *info) {
    // Call the real sysinfo to populate all fields correctly
    int (*real_sysinfo)(struct sysinfo *) = dlsym(RTLD_NEXT, "sysinfo");
    int ret = real_sysinfo(info);
    if (ret == 0) {
        // Override totalram to report 4GB so SQL Server passes its 2GB check
        info->totalram = 4ULL * 1024 * 1024 * 1024;
        info->mem_unit = 1;
    }
    return ret;
}
