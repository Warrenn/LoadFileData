using System;
using LoadFileData.ETLLayer.Constants;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace LoadFileData.ETLLayer
{
    public static class ExceptionHandler
    {
        public static void Dispose(this IDisposable disposable, string policyName = PolicyName.Disposable)
        {
            if (disposable == null)
            {
                return;
            }
            Try(disposable.Dispose, policyName, () => { disposable = null; });
        }

        public static void Try(
            Action tryAction,
            string policyName = PolicyName.Default,
            Action finallyAction = null)
        {
            try
            {
                tryAction();
            }
            catch (Exception exception)
            {
                Exception rethrowException;
                policyName = string.IsNullOrEmpty(policyName) ? PolicyName.Default : policyName;
                if (ExceptionPolicy.HandleException(exception, policyName, out rethrowException))
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
                    Try(finallyAction, policyName);
                }
            }
        }
    }
}
