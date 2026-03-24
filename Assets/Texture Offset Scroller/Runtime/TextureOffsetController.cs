using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpalatoBros.TextureOffsetScrolling
{
    [ExecuteInEditMode]
    public class TextureOffsetController : MonoBehaviour
    {
        [SerializeField] private List<TextureProperty> textureProperties;
        public List<TextureProperty> TextuteProperties => textureProperties;

		private void Awake()
		{
            for (int i = 0; i < textureProperties.Count; i++)
                textureProperties[i].propertyId = Shader.PropertyToID(textureProperties[i].propertyName);
		}

		private void OnEnable()
		{
			for (int i = 0; i < textureProperties.Count; i++)
			{
				if (!textureProperties[i].resetOffsetOnEnable) continue;
				textureProperties[i].ApplyOffset(Vector2.zero);
			}
		}

		private void OnValidate()
		{
			foreach (var item in textureProperties)
			{
				if (string.IsNullOrEmpty(item.propertyName))
					item.propertyName = "_BaseMap";
			}
		}

		private void Update()
		{
            TryUpdateProperties(TextureProperty.UpdateMode.Update);
		}

		private void FixedUpdate()
		{
            TryUpdateProperties(TextureProperty.UpdateMode.FixedUpdate);
		}

		private void LateUpdate()
		{
            TryUpdateProperties(TextureProperty.UpdateMode.LateUpdate);
		}

        private void TryUpdateProperties(TextureProperty.UpdateMode updateMode)
        {
			for (int i = 0; i < textureProperties.Count; i++)
			{
				if (i > textureProperties.Count - 1) break;
				if (textureProperties[i].updateMode != updateMode) continue;
                if (!textureProperties[i].allowRunInEditMode && !Application.isPlaying) continue;

				// Get current offset and get offset multiplier to be used when applying offset.
				Vector2 currentOffset = textureProperties[i].GetOffset(out Vector2 offsetMultiplier);
				Vector2 offsetAmount;

				switch (textureProperties[i].settings.offsetMode)
				{
					case TextureProperty.TextureSettings.OffsetMode.ConstantOffsetOverTime:
						switch (updateMode)
						{
							case TextureProperty.UpdateMode.FixedUpdate:
								if (textureProperties[i].unscaledTime)
									offsetAmount = textureProperties[i].GetOffsetAmountOverTime(Time.fixedUnscaledDeltaTime, offsetMultiplier);
								else
									offsetAmount = textureProperties[i].GetOffsetAmountOverTime(Time.fixedDeltaTime, offsetMultiplier);
								break;
							case TextureProperty.UpdateMode.ManualUpdate:
								offsetAmount = textureProperties[i].GetOffsetAmountOverTime(1, offsetMultiplier);
								break;
							default:
								if (textureProperties[i].unscaledTime)
									offsetAmount = textureProperties[i].GetOffsetAmountOverTime(Time.unscaledDeltaTime, offsetMultiplier);
								else		
									offsetAmount = textureProperties[i].GetOffsetAmountOverTime(Time.deltaTime, offsetMultiplier);
								break;
						}

						textureProperties[i].ApplyOffset(currentOffset + offsetAmount);
						break;
					case TextureProperty.TextureSettings.OffsetMode.PerlinNoiseOverTime:
						switch (updateMode)
						{
							case TextureProperty.UpdateMode.FixedUpdate:
								if (textureProperties[i].unscaledTime)
									offsetAmount = textureProperties[i].GetPerlinNoiseAmountOverTime(Time.fixedUnscaledTime, offsetMultiplier);
								else
									offsetAmount = textureProperties[i].GetPerlinNoiseAmountOverTime(Time.fixedTime, offsetMultiplier);
								break;
							case TextureProperty.UpdateMode.ManualUpdate:
								offsetAmount = textureProperties[i].GetPerlinNoiseAmountOverTime(1, offsetMultiplier);
								break;
							default:
								if (textureProperties[i].unscaledTime)
									offsetAmount = textureProperties[i].GetPerlinNoiseAmountOverTime(Time.unscaledTime, offsetMultiplier);
								else
									offsetAmount = textureProperties[i].GetPerlinNoiseAmountOverTime(Time.time, offsetMultiplier);
								break;
						}

						textureProperties[i].ApplyOffset(offsetAmount);
						break;
					case TextureProperty.TextureSettings.OffsetMode.CurvedOffsetOverTime:
						switch (updateMode)
						{
							case TextureProperty.UpdateMode.FixedUpdate:
								if (textureProperties[i].unscaledTime)				
									offsetAmount = textureProperties[i].GetOffsetAmountCurveOverTime(Time.fixedUnscaledTime, offsetMultiplier);	
								else
									offsetAmount = textureProperties[i].GetOffsetAmountCurveOverTime(Time.fixedTime, offsetMultiplier);	
								break;
							default:
								if (textureProperties[i].unscaledTime)
									offsetAmount = textureProperties[i].GetOffsetAmountCurveOverTime(Time.unscaledTime, offsetMultiplier);	
								else		
									offsetAmount = textureProperties[i].GetOffsetAmountCurveOverTime(Time.time, offsetMultiplier);	
								break;
						}

						textureProperties[i].ApplyOffset(textureProperties[i].ClampOffset(offsetAmount));
						break;
					case TextureProperty.TextureSettings.OffsetMode.RandomUniform:
						offsetAmount = textureProperties[i].GetRandomUniformOffset(offsetMultiplier);
						textureProperties[i].ApplyOffset(currentOffset + offsetAmount);
						break;
					case TextureProperty.TextureSettings.OffsetMode.OnRandomCircle:
						offsetAmount = textureProperties[i].GetRandomOffsetOnCircle(offsetMultiplier);
						textureProperties[i].ApplyOffset(currentOffset + offsetAmount);
						break;
					case TextureProperty.TextureSettings.OffsetMode.InsideRandomCircle:
						offsetAmount = textureProperties[i].GetRandomOffsetInCircle(offsetMultiplier);
						textureProperties[i].ApplyOffset(currentOffset + offsetAmount);
						break;
					default:
						offsetAmount = Vector2.zero;
						textureProperties[i].ApplyOffset(currentOffset + offsetAmount);
						break;
				}
			}
		}
	}

    [Serializable]
    public class TextureProperty
    {
        public Renderer rend;
        public int materialIndex;
		public bool useSharedMaterial;

		public string propertyName = "_BaseMap";
        [HideInInspector] public int propertyId;
		[Space]
        public UpdateMode updateMode;

        public enum UpdateMode
        {
            Update,
            FixedUpdate,
            LateUpdate,
			ManualUpdate
        }

		public bool unscaledTime;

		[Space]
		public Vector2 currentOffset;
		[Space]		
        
		public bool resetOffsetOnEnable;
        public bool multiplySpeedByTiling;
        public bool allowRunInEditMode;

		public TextureSettings settings;

		[Serializable]
		public class TextureSettings
		{
			public OffsetMode offsetMode;
			public enum OffsetMode
			{
				ConstantOffsetOverTime,
				CurvedOffsetOverTime,
				PerlinNoiseOverTime,
				RandomUniform,
				InsideRandomCircle,
				OnRandomCircle
			}

			public OffsetClamping offsetClamping;
			public enum OffsetClamping
			{
				Clamped,
				Repeat
			}

			public Vector2 offsetSpeed;
			public AnimationCurve offsetTimeCurveX;
			public AnimationCurve offsetSpeedCurveY;
			public Vector2 timeOffset;

			public Vector2 offsetRangeX;
			public Vector2 offsetRangeY;
		}

        public Vector2 GetOffset(out Vector2 offsetMultiplier)
        {
			Vector2 currentOffset = Vector2.zero;

			if (useSharedMaterial)
			{
				if (materialIndex > rend.sharedMaterials.Length - 1)
				{
					offsetMultiplier = Vector2.one;
					return currentOffset;
				}

				currentOffset = rend.sharedMaterials[materialIndex].GetTextureOffset(propertyId);

				offsetMultiplier = multiplySpeedByTiling ?
					rend.sharedMaterials[materialIndex].GetTextureScale(propertyId) :
					Vector2.one;
			}
			else
			{
				if (materialIndex > rend.materials.Length - 1)
				{
					offsetMultiplier = Vector2.one;
					return currentOffset;
				}

				currentOffset = rend.materials[materialIndex].GetTextureOffset(propertyId);

				offsetMultiplier = multiplySpeedByTiling ?
					rend.materials[materialIndex].GetTextureScale(propertyId) :
					Vector2.one;
			}

			return currentOffset;
		}

		public Vector2 GetOffsetAmountOverTime(float deltaTime, Vector2 offsetMultiplier)
		{
			return deltaTime * offsetMultiplier * settings.offsetSpeed;
		}

		public Vector2 GetPerlinNoiseAmountOverTime(float currentTime, Vector2 offsetMultiplier)
		{
			return new Vector2(
				Mathf.PerlinNoise(
					settings.timeOffset.x + currentTime * offsetMultiplier.x * settings.offsetSpeed.x, 
					settings.timeOffset.x + currentTime * offsetMultiplier.x * settings.offsetSpeed.x),
				Mathf.PerlinNoise(
					settings.timeOffset.y + currentTime * offsetMultiplier.y * settings.offsetSpeed.y,
					settings.timeOffset.y + currentTime * offsetMultiplier.y * settings.offsetSpeed.y));
		}

		public Vector2 GetOffsetAmountCurveOverTime(float currentTime, Vector2 offsetMultiplier)
		{
			if (multiplySpeedByTiling)
			{
				return new(
					settings.offsetTimeCurveX.Evaluate((currentTime + settings.timeOffset.x) * settings.offsetSpeed.x * offsetMultiplier.x),
					settings.offsetTimeCurveX.Evaluate((currentTime + settings.timeOffset.y) * settings.offsetSpeed.y * offsetMultiplier.y));
			}
			
			return new(
				settings.offsetTimeCurveX.Evaluate((currentTime + settings.timeOffset.x) * settings.offsetSpeed.x),
				settings.offsetTimeCurveX.Evaluate((currentTime + settings.timeOffset.y) * settings.offsetSpeed.y));		
		}

		public Vector2 GetRandomUniformOffset(Vector2 offsetMultiplier)
		{
			Vector2 newOffsetAmount = new(
				Random.Range(-settings.offsetSpeed.x * offsetMultiplier.x, settings.offsetSpeed.x * offsetMultiplier.x),
				Random.Range(-settings.offsetSpeed.y * offsetMultiplier.y, settings.offsetSpeed.y * offsetMultiplier.y));

			return newOffsetAmount;
		}

		public Vector2 GetRandomOffsetOnCircle(Vector2 offsetMultiplier)
		{
			Vector2 onCircle = Random.insideUnitCircle.normalized;
			Vector2 newOffsetAmount = onCircle * offsetMultiplier;
			return newOffsetAmount;
		}

		public Vector2 GetRandomOffsetInCircle(Vector2 offsetMultiplier)
		{
			Vector2 onCircle = Random.insideUnitCircle;
			Vector2 newOffsetAmount = onCircle * offsetMultiplier;
			return newOffsetAmount;
		}

		public Vector2 ClampOffset(Vector2 appliedOffset)
		{
			switch (settings.offsetClamping)
			{
				case TextureSettings.OffsetClamping.Clamped:
					appliedOffset.x = Mathf.Clamp(appliedOffset.x, settings.offsetRangeX.x, settings.offsetRangeX.y);
					appliedOffset.y = Mathf.Clamp(appliedOffset.y, settings.offsetRangeY.x, settings.offsetRangeY.y);
					break;
				case TextureSettings.OffsetClamping.Repeat:
					appliedOffset.x = Mathf.Repeat(appliedOffset.x, settings.offsetRangeX.y);
					appliedOffset.y = Mathf.Repeat(appliedOffset.y, settings.offsetRangeY.y);
					break;
			}

			return appliedOffset;
		}

		public void ApplyOffset(Vector2 newOffset)
		{
			currentOffset = ClampOffset(newOffset);

			if (propertyId == 0)
				propertyId = Shader.PropertyToID(propertyName);

			if (useSharedMaterial)
				rend.sharedMaterials[materialIndex].SetTextureOffset(propertyId, currentOffset);
			else
				rend.materials[materialIndex].SetTextureOffset(propertyId, currentOffset);
		}
    }
}
