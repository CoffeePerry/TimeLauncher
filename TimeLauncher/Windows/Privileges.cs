using System.Security.Principal;

namespace TimeLauncher.Windows {
    internal static class Privileges {
        internal static bool IsAdmin() {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
