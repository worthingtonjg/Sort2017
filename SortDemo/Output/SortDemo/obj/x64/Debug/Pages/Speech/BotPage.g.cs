﻿#pragma checksum "C:\Code\worthingtonjg\SortDemo\Output\SortDemo\Pages\Speech\BotPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7D09D5122F9CB7824BB2BF36134A3309"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SortDemo.Pages.Speech
{
    partial class BotPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.Thinking = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 2:
                {
                    this.BotsCombo = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                    #line 21 "..\..\..\..\Pages\Speech\BotPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ComboBox)this.BotsCombo).SelectionChanged += this.BotsCombo_SelectionChanged;
                    #line default
                }
                break;
            case 3:
                {
                    this.Question = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 4:
                {
                    global::Windows.UI.Xaml.Controls.Button element4 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 30 "..\..\..\..\Pages\Speech\BotPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element4).Click += this.Send_Click;
                    #line default
                }
                break;
            case 5:
                {
                    this.Answer = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

