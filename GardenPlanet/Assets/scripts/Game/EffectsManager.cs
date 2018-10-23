using System;
using System.Collections.Generic;
using UnityEngine;

namespace GardenPlanet
{
    public static class EffectsType
    {
        public static string LOOP_SLEEPING = "looping/Sleeping";
        public static string ONESHOT_STEPDUST = "oneshot/StepDust";
        public static string ONESHOT_CROPDAMAGE = "oneshot/CropDamage";
    }

    public class Effect
    {
        public GameObject effect;
        public Transform followTransform;
        public EffectsManager manager;

        private List<ParticleSystem> systems;
        private bool dying;

        public Effect(GameObject effect, EffectsManager manager)
        {
            this.effect = effect;
            this.manager = manager;
            systems = new List<ParticleSystem>();
            foreach(var comp in effect.GetComponentsInChildren<ParticleSystem>())
                systems.Add(comp);
        }

        public void MultiplySpeed(float factor)
        {
            foreach(var particle in systems)
            {
                var main = particle.main;
                main.simulationSpeed *= factor;
            }
        }

        public void Update()
        {
            if(!effect)
                manager.RemoveEffect(this);
            if(followTransform)
                effect.transform.position = followTransform.transform.position;
            // Waiting for all particles to disappear before destroying
            if(dying)
            {
                foreach(var s in systems)
                    if(s && s.IsAlive())
                        return;
                manager.RemoveEffect(this, immediate:true);
            }
        }

        public void Kill()
        {
            foreach(var s in systems)
                if(s)
                    s.Stop();
            dying = true;
        }
    }

    /*
     * Used to fire off and keep track of effects, effects are typically particle effects
     *
     * CreateEffect will instantiate the effect in question. It will remove itself if it is one-shot otherwise you can
     * use RemoveEffect to remove a created effect.
     */
    public class EffectsManager: MonoBehaviour
    {
        public Dictionary<string, GameObject> preloadedEffects;
        public List<Effect> activeEffects;

        public static EffectsManager CreateEffectsManager()
        {
            var go = new GameObject("Effects Manager");
            return go.AddComponent<EffectsManager>();
        }

        private void Awake()
        {
            activeEffects = new List<Effect>();
            preloadedEffects  = new Dictionary<string, GameObject>();

            // Preload all effects types
            foreach(var prop in typeof(EffectsType).GetFields())
            {
                var name = (string) prop.GetValue(null);
                var path = Consts.PREFAB_PATH_EFFECTS + name;
                preloadedEffects[name] = Resources.Load<GameObject>(path);
                if(!preloadedEffects[name])
                    throw new Exception("Can't find resource for defined effect " + path);
            }
        }

        public Effect CreateEffect(string type, Vector3 location)
        {
            if(!preloadedEffects.ContainsKey(type))
                throw new Exception("Can't find a loaded effect called " + type);
            var newObj = Instantiate(preloadedEffects[type]);
            if(!newObj)
                throw new Exception("Error instantiating " + type);
            newObj.transform.SetParent(gameObject.transform);
            newObj.transform.position = location;
            var effect = new Effect(newObj, this);
            activeEffects.Add(effect);
            return effect;
        }

        public Effect CreateEffect(string type, Transform follow)
        {
            var effect = CreateEffect(type, follow.position);
            effect.followTransform = follow;
            return effect;
        }

        public void RemoveEffect(Effect effect, bool immediate = false)
        {
            if(!immediate)
            {
                effect.Kill();
                return;
            }
            if(effect.effect)
                Destroy(effect.effect);
            activeEffects.Remove(effect);
        }

        private void Update()
        {
            foreach(var effect in new List<Effect>(activeEffects))
                effect.Update();
        }
    }
}