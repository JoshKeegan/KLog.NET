using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("174389bb-f46c-46a4-873a-1f0f3760a142")]

// Allow other Klog.* assemblies to access the internals of the core library
[assembly: InternalsVisibleTo("KLog.Web")]