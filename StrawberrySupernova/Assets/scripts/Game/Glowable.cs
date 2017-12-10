using System;
using UnityEngine;

namespace StrawberryNova
{
	public class Glowable: MonoBehaviour
	{
		static Shader glowShader;

		public string shaderName = "Glow/GlowHiddenNormal";

		float glowSpeed;
		Mesh mesh;
		Color glowColour;
		float glowWidth;
		bool doGlow;
		float glowTimer;
		GameObject glowChild;

		public void Awake()
		{
			if(Glowable.glowShader == null)
				Glowable.glowShader = Shader.Find(shaderName);			
		}

		public void OnEnable()
		{
			var filter = GetComponent<MeshFilter>();
			if(filter != null)
				mesh = filter.mesh;
		}

		public void LateUpdate()
		{
			if(mesh == null  || (glowChild == null && !doGlow && glowTimer <= 0f))
				return;
			// If we're being told to glow
			if(doGlow)
			{
				// Make sure we have a glow child and attach the right shader+material
				if(glowChild == null)
				{
					if(mesh == null)
						return;
					glowChild = new GameObject("Glow Child");
					var childRenderer = glowChild.AddComponent<MeshRenderer>();
					var childFilter = glowChild.AddComponent<MeshFilter>();

					glowChild.transform.parent = transform.parent;

					childRenderer.material = Resources.Load("materials/Glow") as Material;
					childRenderer.material.shader = Glowable.glowShader;
					childFilter.mesh = mesh;
				}

				if(glowTimer < 1f)
					glowTimer += glowSpeed / Time.deltaTime;

				SetGlowChildSettings(glowTimer);
				doGlow = false;
			}
			// Only time we care if we're not is when there's a glow child
			else if(glowChild != null)
			{
				// Make the timer go down to fade out and delete glow child when done
				glowTimer -= glowSpeed / Time.deltaTime;
				if(glowTimer <= 0f)
				{
					Destroy(glowChild);
					glowChild = null;
					glowTimer = 0f;
					return;
				}
				// Set the new glow colour
				SetGlowChildSettings(glowTimer);
			}	
		}

		public void OnDestroy()
		{
			if(glowChild != null)
				Destroy(glowChild);			
		}
			
		public void GlowTo(Color glowColour, float glowWidth, float glowSpeed = 0.001f)
		{
			this.glowColour = glowColour;
			this.glowWidth = glowWidth;
			this.glowSpeed = glowSpeed;
			doGlow = true;
		}

		void SetGlowChildSettings(float time)
		{
			var childRenderer = glowChild.GetComponent<Renderer>().material;
			childRenderer.SetColor("_GlowColor", new Color(glowColour.r, glowColour.g, glowColour.b, time));
			childRenderer.SetFloat("_Outline", glowWidth);
			glowChild.transform.localPosition = transform.localPosition;
			glowChild.transform.localRotation = transform.localRotation;
			glowChild.transform.localScale = transform.localScale;
		}

	}
}

