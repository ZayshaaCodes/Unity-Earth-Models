using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Cookies_TMP_TextSwap : MonoBehaviour {

    [MenuItem("Cookie Utils/Swap Selected TMP")]
    public static void SwapTextToTMP()
    {
        if (Selection.gameObjects != null)
        {

            var sel = Selection.gameObjects;

            Undo.RegisterCompleteObjectUndo(sel, "Swamp Selection To TMP");

            foreach (var selectedGameObject in sel)
            {

                RectTransform rectX = selectedGameObject.transform as RectTransform;
                if (!rectX)
                {
                    continue;
                }

                Vector3 pos = rectX.position;
                Vector2 size = rectX.sizeDelta;
                var oldText = selectedGameObject.GetComponent<Text>();

                if (!oldText)
                {
                    continue;
                }

                var oldTextText = oldText.text;
                var oldAlignment = oldText.alignment;
                var oldColor = oldText.color;
                var oldFrontSize = oldText.fontSize;

                var oldStyle = oldText.fontStyle;

                DestroyImmediate(oldText);

                //creating the new component and converting the values

                var newtext = selectedGameObject.AddComponent<TextMeshProUGUI>();
                
                rectX.position = pos;
                rectX.sizeDelta = size;

                newtext.text = oldTextText;

                var alignOffsetInt = 257;
                if ((int)oldAlignment < 2)
                    alignOffsetInt = 257;
                else if ((int)oldAlignment < 3)
                    alignOffsetInt = 258;
                else if ((int)oldAlignment < 5)
                    alignOffsetInt = 513 - 3;
                else if ((int)oldAlignment < 6)
                    alignOffsetInt = 514 - 3;
                else if ((int)oldAlignment < 8)
                    alignOffsetInt = 1025 - 6;
                else
                    alignOffsetInt = 1026 - 6;

                newtext.alignment = (TextAlignmentOptions)((int)oldAlignment + alignOffsetInt);

                print((int) newtext.alignment);

                newtext.fontSize = oldFrontSize;

                newtext.color = oldColor;

                if ((int) oldStyle < 3)
                {
                    newtext.fontStyle = (FontStyles) oldStyle;
                }
                else
                {
                    newtext.fontStyle = (FontStyles) ((int) FontStyles.Bold + (int) FontStyles.Italic);
                }
            }
        }
    }

}