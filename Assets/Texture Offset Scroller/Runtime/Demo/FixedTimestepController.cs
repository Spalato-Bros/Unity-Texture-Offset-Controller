using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpalatoBros.TextureOffsetScrolling
{
    public class FixedTimestepController : MonoBehaviour
    {
		[SerializeField] private Slider fixedTimestepSlider;
		[SerializeField] private TMP_Text fixedTimestepText;

		private void Awake()
		{
			fixedTimestepSlider.onValueChanged.AddListener(UpdateText);
			fixedTimestepSlider.value = Time.fixedDeltaTime;
		}

		private void UpdateText(float newValue)
		{
			Time.fixedDeltaTime = newValue;
			fixedTimestepText.text = newValue.ToString();
		}
	}
}
