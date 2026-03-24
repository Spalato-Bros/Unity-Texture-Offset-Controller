using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpalatoBros.TextureOffsetScrolling
{
    public class TimeScaleController : MonoBehaviour
    {
        [SerializeField] private Slider timescaleSlider;
        [SerializeField] private TMP_Text timescaleText;

		private void Awake()
		{
			timescaleSlider.onValueChanged.AddListener(UpdateText);
			timescaleSlider.value = Time.timeScale;
		}

		private void UpdateText(float newValue)
		{
			Time.timeScale = newValue;
			timescaleText.text = newValue.ToString();
		}
	}
}
