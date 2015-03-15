using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Parkkis.Services.Utils
{
    /// <summary>
    /// Dialog service
    /// </summary>
    public class DialogService
    {
        /// <summary>
        /// Show message dialog
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="okText"></param>
        /// <param name="cancelText"></param>
        /// <returns></returns>
        public async Task<bool> ShowMessageDialog(string message, string title, string okText, string cancelText)
        {
            bool dialogResult = false;

            var dialog = new MessageDialog(message, title);

            if (!string.IsNullOrWhiteSpace(okText))
            {
                dialog.Commands.Add(new UICommand(okText, command => dialogResult = true));
            }

            if (!string.IsNullOrWhiteSpace(cancelText))
            {
                dialog.Commands.Add(new UICommand(cancelText, command => dialogResult = false));
            }

            await dialog.ShowAsync();

            return dialogResult;
        }
    }
}
