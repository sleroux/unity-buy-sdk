namespace Shopify.Unity {
    using System.Collections.Generic;
    using Shopify.Unity.SDK;
    using MiniJSON;

    /// <summary>
    /// A class describing an error that has occurred within the SDK.
    /// </summary>
    public class ShopifyError : Serializable {
        public static explicit operator ShopifyError(TopLevelResponse response) {
            if (response.HTTPError != null) {
                return new ShopifyError(ShopifyError.ErrorType.HTTP, response.HTTPError);
            }

            if (response.errors != null) {
                return new ShopifyError(ShopifyError.ErrorType.GraphQL, response.errors);
            }

            return null;
        }

        /// <summary>
        /// Type describing the error in more detail.
        /// </summary>
        public enum ErrorType {
            /// <summary>Encountered an HTTP issue or failed connection.</summary>
            HTTP,

            /// <summary>Encountered an internal GraphQL issue with the API.</summary>
            GraphQL,

            /// <summary>UserErrors from the GraphQL API. For example, invalid form fields.</summary>
            UserError,

            /// <summary>Error while handling payment using a native pay method such as Apple Pay.</summary>
            NativePaymentProcessingError
        }

        /// <summary>
        /// Error type for determining what specific error was encountered.
        /// </summary>
        public readonly ErrorType Type;

        /// <summary>
        /// Readable description of the error.
        /// </summary>
        public readonly string Description;

        public ShopifyError(ErrorType type, string description) {
            this.Type = type;
            this.Description = description;
        }

        public override object ToJson() {
            var dict = new Dictionary<string, object>();
            dict["ErrorType"] = Type.ToString();
            dict["Description"] = Description;
            return dict;
        }
    }
}
