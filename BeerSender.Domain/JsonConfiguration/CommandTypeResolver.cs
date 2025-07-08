using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace BeerSender.Domain.JsonConfiguration
{
    /// <summary>
    /// CommandTypeResolver is a class that consists in getting all the types that derive from ICommand and make sure that they'll be serialized and deserialized correctly.
    /// </summary>
    public class CommandTypeResolver : DefaultJsonTypeInfoResolver
    {
        private static readonly List<JsonDerivedType> CommandTypes = new();

        static CommandTypeResolver()
        {
            var commandTypes = typeof(ICommand)
                .Assembly
                .GetTypes()
                .Where(type =>
                       type is { IsClass: true, IsAbstract: false } && typeof(ICommand).IsAssignableFrom(type));

            foreach (var commandType in commandTypes)
            {
                CommandTypes.Add(new JsonDerivedType(commandType, commandType.Name));
            }
        }

        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

            Type commandType = typeof(ICommand);
            if (jsonTypeInfo.Type == commandType)
            {
                var polyOptions = new JsonPolymorphismOptions()
                {
                    TypeDiscriminatorPropertyName = "$command-type",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = System.Text.Json.Serialization.JsonUnknownDerivedTypeHandling.FallBackToBaseType
                };
                foreach (var jsonDerivedType in CommandTypes)
                {
                    polyOptions.DerivedTypes.Add(jsonDerivedType);
                }

                jsonTypeInfo.PolymorphismOptions = polyOptions;
            }

            return jsonTypeInfo; 
        }
    }
}
