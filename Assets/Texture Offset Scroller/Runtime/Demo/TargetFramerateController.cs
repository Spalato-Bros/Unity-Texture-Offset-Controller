using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpalatoBros.TextureOffsetScrolling
{
    public class TargetFramerateController : MonoBehaviour
    {
		[SerializeField] private Slider targetFramerateSlider;
		[SerializeField] private TMP_Text targetFramerateText;

		private void Awake()
		{
			Application.targetFrameRate = 60;

			targetFramerateSlider.onValueChanged.AddListener(UpdateText);
			targetFramerateSlider.value = Application.targetFrameRate;
		}

		private void UpdateText(float newValue)
		{
			Application.targetFrameRate = Mathf.RoundToInt(newValue);
			targetFramerateText.text = newValue.ToString();
		}
	}
}
