package com.shopify.unity.buy;

import android.app.Fragment;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.Nullable;

import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.wallet.MaskedWallet;
import com.google.android.gms.wallet.Wallet;
import com.google.android.gms.wallet.WalletConstants;
import com.shopify.buy3.pay.PayCart;
import com.shopify.buy3.pay.PayHelper;

import java.util.ArrayList;
import java.util.List;

public class UnityAndroidPayFragment extends Fragment implements GoogleApiClient.ConnectionCallbacks {

    private static final String EXTRA_UNITY_DELEGATE_OBJECT_NAME = "unityDelegateObjectName";
    private static final String EXTRA_PAY_CART = "payCart";
    private static final String EXTRA_COUNTRY_CODE = "countryCode";
    private static final String EXTRA_ANDROID_PAY_ENVIRONMENT = "androidPayEnvironment";
    private static final String EXTRA_PUBLIC_KEY = "publicKey";

    private PayCart cart;
    private String unityDelegateObjectName;
    private String countryCode;
    private String androidPublicKey;
    private int androidPayEnvironment;
    private GoogleApiClient googleApiClient;

    private MaskedWallet maskedWallet;

    static final class UnityAndroidPayFragmentBuilder {
        private static final String[] requiredExtras = {
            EXTRA_UNITY_DELEGATE_OBJECT_NAME,
            EXTRA_PAY_CART,
            EXTRA_COUNTRY_CODE,
            EXTRA_ANDROID_PAY_ENVIRONMENT,
            EXTRA_PUBLIC_KEY
        };

        private Bundle bundle;

        UnityAndroidPayFragmentBuilder() {
            bundle = new Bundle();
        }

        UnityAndroidPayFragmentBuilder setUnityDelegateObjectName(String name) {
            bundle.putString(EXTRA_UNITY_DELEGATE_OBJECT_NAME, name);
            return this;
        }

        UnityAndroidPayFragmentBuilder setPayCart(PayCart cart) {
            bundle.putParcelable(EXTRA_PAY_CART, cart);
            return this;
        }

        UnityAndroidPayFragmentBuilder setCountryCode(String countryCode) {
            bundle.putString(EXTRA_COUNTRY_CODE, countryCode);
            return this;
        }

        UnityAndroidPayFragmentBuilder setEnvironment(int environment) {
            bundle.putInt(EXTRA_ANDROID_PAY_ENVIRONMENT, environment);
            return this;
        }

        UnityAndroidPayFragmentBuilder setPublicKey(String key) {
            bundle.putString(EXTRA_PUBLIC_KEY, key);
            return this;
        }

        UnityAndroidPayFragment build() {
            checkBundleContainsRequiredExtras();
            UnityAndroidPayFragment fragment = new UnityAndroidPayFragment();
            fragment.setArguments(bundle);
            return fragment;
        }

        private void checkBundleContainsRequiredExtras() {
            List<String> missingExtras = new ArrayList<>();
            for (String extra : requiredExtras) {
                if (bundle.get(extra) == null) {
                    missingExtras.add(extra);
                }
            }

            if (missingExtras.size() > 0) {
                throw new IllegalArgumentException("Missing required bundle extras: " + missingExtras.toString());
            }
        }
    }

    public static UnityAndroidPayFragmentBuilder builder() {
        return new UnityAndroidPayFragmentBuilder();
    }

    @Override
    public void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Bundle bundle = getArguments();
        if (bundle != null) {
            unityDelegateObjectName = bundle.getString(EXTRA_UNITY_DELEGATE_OBJECT_NAME);
            cart = bundle.getParcelable(EXTRA_PAY_CART);
            countryCode = bundle.getString(EXTRA_COUNTRY_CODE);
            androidPayEnvironment = bundle.getInt(EXTRA_ANDROID_PAY_ENVIRONMENT,
                    WalletConstants.ENVIRONMENT_TEST);
            androidPublicKey = bundle.getString(EXTRA_PUBLIC_KEY);
        }

        // Don't recreate this fragment during configuration change.
        setRetainInstance(true);

        googleApiClient = new GoogleApiClient.Builder(this.getActivity())
                .addApi(Wallet.API, new Wallet.WalletOptions.Builder()
                        .setEnvironment(androidPayEnvironment)
                        .setTheme(WalletConstants.THEME_DARK)
                        .build())
                .addConnectionCallbacks(this)
                .build();
    }

    @Override
    public void onStart() {
        super.onStart();
        googleApiClient.connect();
    }

    @Override
    public void onStop() {
        googleApiClient.disconnect();
        super.onStop();
    }

    @Override
    public void onConnected(@Nullable Bundle bundle) {
        // On connection, request for a masked wallet.
        PayHelper.requestMaskedWallet(googleApiClient, cart, androidPublicKey);
    }

    @Override
    public void onConnectionSuspended(int i) { }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        PayHelper.handleWalletResponse(requestCode, resultCode, data, new PayHelper.WalletResponseHandler() {

            @Override
            public void onMaskedWallet(MaskedWallet maskedWallet) {
                super.onMaskedWallet(maskedWallet);

                // TODO: Send MaskedWallet information back to Unity to update that state
                UnityAndroidPayFragment.this.maskedWallet = maskedWallet;

                // TODO: Got a wallet - show the confirmation fragment
            }

            @Override
            public void onWalletError(int requestCode, int errorCode) {
                // TODO: Error in wallet request - bubble error to Unity?
            }

            @Override
            public void onWalletRequestCancel(int requestCode) {
                // TODO: We probably want to cancel out of the Android Pay stuff and tell Unity!
            }
        });
    }
}