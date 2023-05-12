using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.SceneManagement;

namespace FG {
    public class BuildPostprocessor {
        [PostProcessSceneAttribute(0)]
        public static void OnPostprocessBuild() {
        Scene scene = SceneManager.GetActiveScene();

#if FG_DEMO
            GameObject[] desktopObjects = scene.GetRootGameObjects();
             foreach (GameObject go in desktopObjects) {
                 DestroyChildObjectsWithTag(go.transform, "FullVersionOnly");
             }
            
            foreach (GameObject go in desktopObjects) {
                 DestroyChildObjectsWithTag(go.transform, "InGameEditor");
             }
#endif

#if (UNITY_IOS || UNITY_ANDROID)
            GameObject[] desktopObjects = scene.GetRootGameObjects();
             foreach (GameObject go in desktopObjects) {
                 DestroyChildObjectsWithTag(go.transform, "DesktopOnly");
             }
             
             foreach (GameObject go in desktopObjects) {
                 DestroyChildObjectsWithTag(go.transform, "InGameEditor");
             }
#else
            GameObject[] mobileObjects = scene.GetRootGameObjects();
            foreach (GameObject go in mobileObjects) {
                DestroyChildObjectsWithTag(go.transform, "MobileOnly");
            }
#endif
        }

        private static void DestroyChildObjectsWithTag(Transform transform, in string tag) {
            for (int i = transform.childCount - 1; i >= 0; i--) {
                if (transform.GetChild(i).CompareTag(tag)) {
                    GameObject.DestroyImmediate(transform.GetChild(i).gameObject, false);
                    //break;
                }
                else {
                    DestroyChildObjectsWithTag(transform.GetChild(i), tag);
                }
            }
            
            foreach (Transform transGo in transform) {
                if (transGo.CompareTag(tag)) {
                    GameObject.DestroyImmediate(transGo.gameObject, false);
                    //break;
                }
                else {
                    DestroyChildObjectsWithTag(transGo, tag);
                }
            }
        }
    }
}