﻿#region License
/*
    PloobsEngine Game Engine Version 0.3 Beta
    Copyright (C) 2011  Ploobs

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAnimation;
using XNAnimation.Controllers;
using PloobsEngine.Utils;
using PloobsEngine.Engine.Logger;

namespace PloobsEngine.Modelo.Animation
{
    /// <summary>
    /// Animated Controller concrete
    /// </summary>
    public class AnimatedController : IAnimatedController
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedController"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="StartAnimationName">Start name of the animation.</param>
        /// <param name="changeOnlyWhenDifferentAnimation">if set to <c>true</c> [change the current animationonly only when different animation is set].</param>
        public AnimatedController(IAnimatedModel model, String StartAnimationName = null,bool changeOnlyWhenDifferentAnimation = true)            
        {
            skinnedModel = model.GetAnimation() as SkinnedModel;
            AnimationController = new AnimationController(skinnedModel.SkeletonBones);            
            this.StartAnimationName = StartAnimationName;
            this.actualAnimation = StartAnimationName;
            if(StartAnimationName != null)
                AnimationController.StartClip(skinnedModel.AnimationClips[StartAnimationName]);
            this.changeOnlyWhenDifferentAnimation = changeOnlyWhenDifferentAnimation;

        }

        private bool changeOnlyWhenDifferentAnimation = true;
        private string actualAnimation;
        private SkinnedModel skinnedModel;                
        private String StartAnimationName = null;
        private IDictionary<String, String> _actionAnimation = new Dictionary<string, String>();
        private double transitionBetweenAnimationTimeInSeconds = 1;

        /// <summary>
        /// Gets or sets the transition between animation time in seconds.
        /// </summary>
        /// <value>
        /// The transition between animation time in seconds.
        /// </value>
        public double TransitionBetweenAnimationTimeInSeconds
        {
            get { return transitionBetweenAnimationTimeInSeconds; }
            set { transitionBetweenAnimationTimeInSeconds = value; }
        }

        /// <summary>
        /// Gets or sets the animation speed.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public float Speed
        {
            get
            {
                return AnimationController.Speed;
            }
            set
            {
                this.AnimationController.Speed = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in loop.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is loop; otherwise, <c>false</c>.
        /// </value>
        public bool isLoop
        {
            get
            {
                return AnimationController.LoopEnabled;
            }
            set
            {
                this.AnimationController.LoopEnabled = value;
            }
        }

        /// <summary>
        /// Mapps one action to animation.
        /// When using behaviors for example
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="animation">The animation.</param>
        public void MappActionAnimation(String actionName, String animation)
        {
            _actionAnimation.Add(actionName, animation);
        }


        #region IAnimatedController Members

        /// <summary>
        /// Transforms the bone.
        /// </summary>
        /// <param name="boneName">Name of the bone.</param>
        /// <param name="rot">The rot.</param>
        public void TransformBone(String boneName,Quaternion rot)
        {
            AnimationController.SetBoneController(boneName, rot);
        }

        /// <summary>
        /// Gets the bone absolute transform.
        /// </summary>
        /// <param name="boneName">Name of the bone.</param>
        /// <returns></returns>
        public Matrix GetBoneAbsoluteTransform(String boneName)
        {
            return AnimationController.GetBoneAbsoluteTransform(boneName);
        }

        public AnimationController AnimationController
        {
            internal set;
            get;
        }

        /// <summary>
        /// Changes the animation.
        /// </summary>
        /// <param name="animationName">Name of the animation.</param>
        /// <param name="mode">The interpolation mode.</param>
        public void ChangeAnimation(string animationName, AnimationChangeMode mode)
        {
            if (changeOnlyWhenDifferentAnimation)
            {
                if (animationName != actualAnimation)
                {
                    AnimationController.CrossFade(
                            skinnedModel.AnimationClips[animationName],
                            TimeSpan.FromSeconds(transitionBetweenAnimationTimeInSeconds));
                    actualAnimation = animationName;
                }
            }
            else
            {
                AnimationController.CrossFade(
                     skinnedModel.AnimationClips[animationName],
                     TimeSpan.FromSeconds(transitionBetweenAnimationTimeInSeconds));
                actualAnimation = animationName;

            }            
        }        

        #endregion

        

        #region IAnimatedController Members


        /// <summary>
        /// Updates the controller.
        /// CAlled by the API
        /// </summary>
        /// <param name="gt">The gt.</param>
        public void Update(Microsoft.Xna.Framework.GameTime gt)
        {
            AnimationController.Update(gt.ElapsedGameTime, Matrix.Identity);
        }

        #endregion
 

        #region IAnimatedController Members

        /// <summary>
        /// Changes the interpolation mode.
        /// </summary>
        /// <param name="im">The im.</param>
        public void ChangeInterpolationMode(AnimationInterpolationMode im)
        {
            if (im == AnimationInterpolationMode.No_Interpolation)
            {

                AnimationController.TranslationInterpolation = InterpolationMode.None;
                AnimationController.OrientationInterpolation = InterpolationMode.None;
                AnimationController.ScaleInterpolation = InterpolationMode.None;
            }
            else if (im == AnimationInterpolationMode.Linear_Interpolation)
            {

                AnimationController.TranslationInterpolation = InterpolationMode.Linear;
                AnimationController.OrientationInterpolation = InterpolationMode.Linear;
                AnimationController.ScaleInterpolation = InterpolationMode.Linear;
            }
            else if (im == AnimationInterpolationMode.Cubic_Interpolation)
            {

                AnimationController.TranslationInterpolation = InterpolationMode.Cubic;
                AnimationController.OrientationInterpolation = InterpolationMode.Linear;
                AnimationController.ScaleInterpolation = InterpolationMode.Cubic;
            }
            else if (im == AnimationInterpolationMode.Spherical_Interpolation)
            {
                AnimationController.TranslationInterpolation = InterpolationMode.Linear;
                AnimationController.OrientationInterpolation = InterpolationMode.Spherical;
                AnimationController.ScaleInterpolation = InterpolationMode.Linear;

            }
        }

        #endregion

        #region IAnimatedController Members


        /// <summary>
        /// Gets the bone transformations.
        /// </summary>
        /// <returns></returns>
        public Matrix[] GetBoneTransformations()
        {
            return AnimationController.SkinnedBoneTransforms;
        }        

        #endregion

#if !WINDOWS_PHONE
        #region ISerializable Members

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            ActiveLogger.LogMessage("Serialization not yet implemented", LogLevel.Warning);
        }

        #endregion
#endif
    }
}
