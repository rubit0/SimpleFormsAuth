using System.Collections.Generic;

namespace Rubito.XamarinForms.SimpleAuth.Models
{
    public class CredentialsValidationResult
    {
        public bool IsValid { get; private set; }
        public IEnumerable<string> ErrorMessages { get; private set; }

        public CredentialsValidationResult(bool isValid, IEnumerable<string> errorMessages)
        {
            IsValid = isValid;
            ErrorMessages = errorMessages;
        }

        public string GetFormatedErrors()
        {
            if (ErrorMessages == null)
                return string.Empty;

            var messages = string.Empty;

            foreach (var errorMessage in ErrorMessages)
            {
                if (string.IsNullOrEmpty(messages))
                    messages += errorMessage;
                else
                    messages += "\n" + errorMessage;
            }

            return messages;
        }
    }
}