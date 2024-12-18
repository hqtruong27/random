namespace Shared.Factories;

public static class DynamicDelegateFactory
{
    public static Delegate Create(string httpMethod, Type inputType, ISender sender)
    {
        if (HttpMethodHelper.IsHttpMethodSupported(httpMethod))
        {
            return async (HttpContext context) =>
            {
                var instance = await InstanceCreator.CreateInstance(inputType, context);
                return await sender.Send(instance);
            };
        }

        return inputType.CreateDelegateBasedOnRequestType(sender);
    }

    private static Delegate CreateDelegateBasedOnRequestType(this Type inputType, ISender sender)
    {
        var iRequestInterface = inputType.GetIRequestInterface();
        if (iRequestInterface != null)
        {
            return DynamicHandlerFactory.CreateForGenericRequest(iRequestInterface, inputType, sender);
        }

        if (inputType.ImplementsInterface<IRequest>())
        {
            return DynamicHandlerFactory.CreateForNonGenericRequest(inputType, sender);
        }

        throw new InvalidOperationException(
            $"{inputType.Name} does not implement IRequest or IRequest<>"
            );
    }
}

