using System;
using System.Configuration;
using System.Diagnostics;
using LoadFileData.Constants;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;

namespace LoadFileData
{
    public static class ExceptionHandler
    {
        private static readonly ExceptionHandlingSettings Settings =
            ConfigurationManager.GetSection(ExceptionHandlingSettings.SectionName) as ExceptionHandlingSettings;

        public static void Dispose(this IDisposable disposable, string policyName = PolicyName.Disposable)
        {
            if (disposable == null)
            {
                return;
            }
            Try(disposable.Dispose, policyName, () => { disposable = null; });
        }

        public static bool HandleException(Exception exception, string policyName = null)
        {
            Exception rethrowException;
            return HandleException(exception, policyName, out rethrowException);
        }

        public static bool HandleException(Exception exception, string policyName, out Exception rethrowException)
        {
            rethrowException = null;
            if (!string.IsNullOrEmpty(policyName) &&
                (Settings != null) &&
                (Settings.ExceptionPolicies.Contains(policyName)))
            {
                return ExceptionPolicy.HandleException(exception, policyName, out rethrowException);
            }
            Trace.TraceError(exception.Message);
            return false;
        }

        public static void Try(
            Action tryAction,
            string policyName = null,
            Action finallyAction = null)
        {
            try
            {
                tryAction();
            }
            catch (Exception exception)
            {
                Exception rethrowException;
                if (HandleException(exception, policyName, out rethrowException))
                {
                    if (rethrowException == null)
                    {
                        throw;
                    }
                    throw rethrowException;
                }
            }
            finally
            {
                if (finallyAction != null)
                {
                    Try(finallyAction);
                }
            }
        }
    }
}
