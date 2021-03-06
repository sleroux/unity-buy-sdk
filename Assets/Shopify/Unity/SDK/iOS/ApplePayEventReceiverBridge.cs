namespace Shopify.Unity.SDK.iOS {
    using System.Collections.Generic;
    using System.Collections;
    using UnityEngine;
    public partial class ApplePayEventReceiverBridge : MonoBehaviour { }

#if UNITY_IOS
    public partial class ApplePayEventReceiverBridge : IApplePayEventReceiver {
        public IApplePayEventReceiver Receiver;

        public void UpdateSummaryItemsForShippingIdentifier(string serializedMessage) {
            Receiver.UpdateSummaryItemsForShippingIdentifier(serializedMessage);
        }

        public void UpdateSummaryItemsForShippingContact(string serializedMessage) {
            Receiver.UpdateSummaryItemsForShippingContact(serializedMessage);
        }

        public void FetchApplePayCheckoutStatusForToken(string serializedMessage) {
            Receiver.FetchApplePayCheckoutStatusForToken(serializedMessage);
        }

        public void DidFinishCheckoutSession(string serializedMessage) {
            Receiver.DidFinishCheckoutSession(serializedMessage);
        }
    }
#endif
}
