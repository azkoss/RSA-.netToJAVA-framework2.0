﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:2.0.50727.8794
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Test.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALjwSaTjSWprtYB3YzfalpVbkV0IScu65HPgjPbEIrsgtM/sQYmd2QGyrT7fcWxbHeHgj4nqZq3RFRyiM6h/SQGtahz5Xt/FrMxISxjnxHJR2emSOAVQk2j/cFyUm8okBq3taPpkO4C8RrjO8WpoQl0c9m+4faF1hSb6LFwOP6nBAgMBAAECgYEAkQOx635hqfYNW0/CWCCp5THo+RcvrnW8/3P7dN/1D+Ckh0mNVliUufUeTeetq7aC5wRL6WwI2ZDSSiKR+TTdzAH79ADeukYF8t7xlpIXUBWdf/HPqQTdkpVf+8xguEWbZay3FhUoMbDrNwrHJnVeJII23m71FxpQl2R0xIzjvwECQQDn/9e/MSMkrQzyuOl+ncWvRUHsm15NkfGv1MU+jzd9JfGJPX6pj/eZY2ixG7i1daqFd0qhsPORn9RuiknrlfyxAkEAzBIcZNUiGHjNEvbw0YLsRohVIow2w3KIDKg2tL3x4jm3TfP79GK4YZ3qEXq5SGjJfEkENhdl7IFq7iDyjHiCEQJAU3rOCTAHM0VJqXU8H6Fp5r2HETp+3m6rhteK+g3Sq6ehl/6WuzDgqUMKAuC0wCbM6yWXp0LjAf1/FR+RpcHcoQJAa+DjG7bECHXLy0u5sLfqWbr2boX66UVhgHdoPBHxjar/KPli5yVM3WXSeB0NV6b1ZHtg+4tQ+T7NHUdTkUifUQJBAOQXXaEHI53epuRA6kWki8QVRU1nnV2GqqYJqAZVbReFonQHZvbSImeGAv2IBjEIHgjPQHgLzwoCTSYnpAry9Kc=")]
        public string privateKey {
            get {
                return ((string)(this["privateKey"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC48Emk40lqa7WAd2M32paVW5FdCEnLuuRz4Iz2xCK7I" +
            "LTP7EGJndkBsq0+33FsWx3h4I+J6mat0RUcojOof0kBrWoc+V7fxazMSEsY58RyUdnpkjgFUJNo/3Bcl" +
            "JvKJAat7Wj6ZDuAvEa4zvFqaEJdHPZvuH2hdYUm+ixcDj+pwQIDAQAB")]
        public string publicKey {
            get {
                return ((string)(this["publicKey"]));
            }
            set {
                this["publicKey"] = value;
            }
        }
    }
}
