using UnityEngine;
using UnityEngine.SceneManagement;
using System;

#if UNITY_ANDROID
using UnityEngine.Purchasing;
#endif

namespace FG {
    public class MobileStore : MonoBehaviour
#if UNITY_ANDROID
        , IStoreListener
#endif
    {
        [SerializeField] private GameObject _buyButton;

#if UNITY_ANDROID
        private static IStoreController _storeController;
        private static IExtensionProvider _storeExtensionProvider;

        private const string FullVersion = "com.farewellgames.boulderspuzzle.fullgame";

        public static bool IsInitialized => _storeController != null && _storeExtensionProvider != null;

        public void OnGetLevelsButtonClick() {
            if (GameSettings.Instance.BoughtFullVersion) {
                SceneManager.LoadScene(3);
            }
            else {
                BuyProductID(FullVersion);
            }
        }

        private void Start() {
            if (_storeController == null) {
                InitializePurchasing();
            }
        }

        private void UpdatePreviousPurchases() {
            if (_storeController.products.WithID(FullVersion).hasReceipt && !GameSettings.Instance.BoughtFullVersion) {
                GameSettings.Instance.BoughtFullVersion = true;
                _buyButton.SetActive(!GameSettings.Instance.BoughtFullVersion);
                GameSettings.SaveSettings();
            }
        }

        public void InitializePurchasing() {
            if (IsInitialized) {
                return;
            }
            
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(FullVersion, ProductType.NonConsumable);
            UnityPurchasing.Initialize(this, builder);
        }
        
        public void OnInitializeFailed(InitializationFailureReason error) {
            // Todo play unsuccessful sound
        }
        
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
            Debug.Log($"init {controller}");
            _storeController = controller;
            _storeExtensionProvider = extensions;
            
            UpdatePreviousPurchases();
        }
        
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
            if (String.Equals(args.purchasedProduct.definition.id, FullVersion, StringComparison.Ordinal)) {
                Debug.Log($"ProcessPurchase: PASS. Product: '{args.purchasedProduct.definition.id}'");
                GameSettings.Instance.BoughtFullVersion = true;
                GameSettings.SaveSettings();
            }

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p) {
            // Todo play unsuccessful sound
        }

        private void BuyProductID(in string productID) {
            if (!IsInitialized) {
                return;
            }
            
            Product product = _storeController.products.WithID(productID);
            if (product != null && product.availableToPurchase) {
                _storeController.InitiatePurchase(product);
            }
        }
        
        private void Awake() {
            _buyButton.SetActive(!GameSettings.Instance.BoughtFullVersion);
        }
#endif
    }
}
