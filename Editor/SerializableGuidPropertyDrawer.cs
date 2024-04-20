using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SH.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidPropertyDrawer : PropertyDrawer
    {
        private Color _defaultColor = new Color32(210, 210, 210, 255);
        private Color _badColor = Color.red;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var arrayProperty = property.FindPropertyRelative("_guid");
            var field = new TextField($"{property.displayName} (Guid)");

            field.SetValueWithoutNotify(GetFormatedGuid(arrayProperty));
            field.RegisterValueChangedCallback(x => ParseGuid(arrayProperty, field, x.newValue));
            field.TrackPropertyValue(arrayProperty, x => TrackGuid(x, field));

            field.AddToClassList("unity-base-field__aligned");
            field.Add(new Button(() => CopyGuid(field)) { text = "Copy" });
            field.Add(new Button(() => PasteGuid(arrayProperty, field)) { text = "Paste" });
            field.Add(new Button(() => SetNewGuid(arrayProperty)) { text = "New" });

            return field;
        }

        private void CopyGuid(TextField field)
        {
            GUIUtility.systemCopyBuffer = field.value;
        }

        private void PasteGuid(SerializedProperty property, TextField field)
        {
            ParseGuid(property, field, GUIUtility.systemCopyBuffer);
        }

        private void SetNewGuid(SerializedProperty property)
        {
            SetGuid(property, Guid.NewGuid());
        }

        private void TrackGuid(SerializedProperty property, TextField field)
        {
            field.SetValueWithoutNotify(GetFormatedGuid(property));
            var inputElement = field.Q<TextElement>(className: "unity-text-element--inner-input-field-component");
            inputElement.style.color = _defaultColor;
        }

        private string GetFormatedGuid(SerializedProperty property)
        {
            byte[] array = new byte[property.arraySize];
            for (int i = 0; i < property.arraySize; i++)
            {
                array[i] = (byte)property.GetArrayElementAtIndex(i).boxedValue;
            }
            Guid guid = new(array);
            return guid.ToString().ToUpper();
        }

        private void ParseGuid(SerializedProperty property, TextField field, string formated)
        {
            var inputElement = field.Q<TextElement>(className: "unity-text-element--inner-input-field-component");
            if (Guid.TryParseExact(formated, "D", out Guid guid)
            || Guid.TryParseExact(formated, "N", out guid))
            {
                inputElement.style.color = _defaultColor;
                SetGuid(property, guid);
            }
            else
            {
                field.SetValueWithoutNotify(formated);
                inputElement.style.color = _badColor;
            }
        }

        private void SetGuid(SerializedProperty property, Guid guid)
        {
            var length = property.arraySize;
            var array = guid.ToByteArray();
            for (int i = 0; i < length; i++)
            {
                property.GetArrayElementAtIndex(i).boxedValue = array[i];
            }
            if (property.serializedObject.hasModifiedProperties)
            {
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.SetIsDifferentCacheDirty();
                property.serializedObject.UpdateIfRequiredOrScript();
            }
        }
    }
}