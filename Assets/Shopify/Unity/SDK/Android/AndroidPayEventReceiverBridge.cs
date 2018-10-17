#if UNITY_ANDROID
namespace Shopify.Unity.SDK.Android {
    using System.Collections.Generic;
    using System.Collections;

    using UnityEngine;
    public partial class AndroidPayEventReceiverBridge : MonoBehaviour { }

    public partial class AndroidPayEventReceiverBridge : IAndroidPayEventReceiver {
        public IAndroidPayEventReceiver Receiver;

        public void OnCanCheckoutWithAndroidPayResult(string serializedMessage) {
            Receiver.OnCanCheckoutWithAndroidPayResult(serializedMessage);
        }

        public void OnUpdateShippingAddress(string serializedMessage) {
            Receiver.OnUpdateShippingAddress(serializedMessage);
        }

        public void OnUpdateShippingLine(string serializedMessage) {
            Receiver.OnUpdateShippingLine(serializedMessage);
        }

        public void OnConfirmCheckout(string serializedMessage) {
            Receiver.OnConfirmCheckout(serializedMessage);
        }

        public void OnError(string serializedMessage) {
            Receiver.OnError(serializedMessage);
        }

        public void OnCancel(string serializedMessage) {
            Receiver.OnCancel(serializedMessage);
        }
    }
}
#endif
