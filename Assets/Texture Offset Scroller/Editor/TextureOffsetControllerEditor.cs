using UnityEditor;
using UnityEngine;

namespace SpalatoBros.TextureOffsetScrolling
{
    [CustomEditor(typeof(TextureOffsetController))]
    public class TextureOffsetControllerEditor : Editor
    {
		private void OnEnable()
		{
			EditorApplication.update += OnEditorUpdate;
		}

		private void OnDisable()
		{
			EditorApplication.update -= OnEditorUpdate;
		}

		private void OnEditorUpdate()
		{
			Repaint();
		}
    }
}
