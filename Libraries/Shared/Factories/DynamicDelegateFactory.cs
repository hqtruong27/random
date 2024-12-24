namespace Shared.Factories;

public static class DynamicDelegateFactory
{
    public static Delegate Create(string httpMethod, Type inputType, IServiceProvider serviceProvider)
    {
        if (HttpMethodHelper.IsHttpMethodSupported(httpMethod))
        {
            return async (HttpContext context) =>
            {
                var sender = context.RequestServices.GetRequiredService<ISender>();
                var instance = await InstanceCreator.CreateInstance(inputType, context);
                return await sender.Send(instance);
            };
        }

        return inputType.CreateDelegateBasedOnRequestType(serviceProvider);
    }

    private static Delegate CreateDelegateBasedOnRequestType(this Type inputType, IServiceProvider serviceProvider)
    {
        var iRequestInterface = inputType.GetIRequestInterface();
        if (iRequestInterface != null)
        {
            return DynamicHandlerFactory.CreateForGenericRequest(iRequestInterface, inputType, serviceProvider);
        }

        if (inputType.ImplementsInterface<IRequest>())
        {
            return DynamicHandlerFactory.CreateForNonGenericRequest(inputType, serviceProvider);
        }

        throw new InvalidOperationException(
            $"{inputType.Name} does not implement IRequest or IRequest<>"
            );
    }
}