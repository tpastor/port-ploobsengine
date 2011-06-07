﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;
using PloobsEngine.Commands;
using PloobsEngine.Engine;
using PloobsEngine.SceneControl;

namespace PloobsEngine.Components
{
    /// <summary>
    /// Handles the Components
    /// </summary>
    public class ComponentManager 
    {        
        private IDictionary<String, IComponent> _comps = new Dictionary<String, IComponent>();
        private List<IComponent> _updateables = new List<IComponent>();        
        private List<IComponent> _preDrawables = new List<IComponent>();
        private List<IComponent> _posDrawables = new List<IComponent>();
        private List<IComponent> _posWithDepthDrawables = new List<IComponent>();
        private List<IComponent> updateAUX = new List<IComponent>();
        private GraphicInfo GraphicInfo;
        GraphicFactory factory;



        public ComponentManager(GraphicInfo GraphicInfo, GraphicFactory factory)
        {
            this.GraphicInfo = GraphicInfo;
            this.factory = factory;
        }

        
        /// <summary>
        /// Updates 
        /// </summary>
        /// <param name="gt">The gt.</param>
        internal void Update(GameTime gt)
        {   
            updateAUX.Clear();
            foreach (var item in _updateables)
	        {
        		 updateAUX.Add(item);
	        }

            while (updateAUX.Count > 0)
            {
                IComponent comp = updateAUX[updateAUX.Count - 1];
                comp.iUpdate(gt);                
                updateAUX.Remove(comp);                
            }
                        
        }

        /// <summary>
        /// Gets all the pre draw components.
        /// </summary>
        /// <returns></returns>
        internal IList<IComponent> GetPreDraw()
        {
            return _preDrawables;
        }

        /// <summary>
        /// Gets all the pos draw components.
        /// </summary>
        /// <returns></returns>
        internal IList<IComponent> GetPosDraw()
        {
            return _posDrawables;
        }

        /// <summary>
        /// Gets the pos with Depth draw.
        /// </summary>
        /// <returns></returns>
        internal IList<IComponent> GetPosWithDepthDraw()
        {
            return _posWithDepthDrawables;
        }
        

        /// <summary>
        /// Draw the Pre Draw components
        /// </summary>
        /// <param name="gt">The gt.</param>
        /// <param name="activeView">The active view.</param>
        /// <param name="activeProjection">The active projection.</param>
        internal void PreDraw(RenderHelper render,GameTime gt, Matrix activeView, Matrix activeProjection)
        {
            foreach (IComponent item in _preDrawables)
            {
                item.iPreDraw(render,gt,activeView,activeProjection);
            }
        }
        /// <summary>
        /// Draw the Afters draw.
        /// </summary>
        /// <param name="gt">The gt.</param>
        /// <param name="activeView">The active view.</param>
        /// <param name="activeProjection">The active projection.</param>
        internal void AfterDraw(RenderHelper render,GameTime gt, Matrix activeView, Matrix activeProjection)
        {
            foreach (IComponent item in _posDrawables)
            {
                item.iAfterDraw(render,gt,activeView,activeProjection);
            }
        }

        /// <summary>
        /// Draw the Afters with depth draw.
        /// </summary>
        /// <param name="render">The render.</param>
        /// <param name="gt">The gt.</param>
        /// <param name="activeView">The active view.</param>
        /// <param name="activeProjection">The active projection.</param>
        internal void PosWithDepthDraw(RenderHelper render, GameTime gt, Matrix activeView, Matrix activeProjection)
        {
            foreach (IComponent item in _posWithDepthDrawables)
            {
                item.iPosWithDepthDraw(render, gt, activeView, activeProjection);
            }
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="engine">The engine.</param>
        internal void LoadContent(ref GraphicInfo GraphicInfo)
        {
            foreach (IComponent item in _comps.Values)
            {
                item.iLoadContent(GraphicInfo,factory);
            }
        }

        /// <summary>
        /// Determines whether the component is already added
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <returns>
        ///   <c>true</c> if the specified component name has component; otherwise, <c>false</c>.
        /// </returns>
        public bool HasComponent(String componentName)
        {
            return _comps.ContainsKey(componentName);            
        }

        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <param name="comp">The comp.</param>
        /// <returns>FALSE if the component already exist, false otherwise </returns>
        public bool AddComponent(IComponent comp)
        {
            if (HasComponent(comp.getMyName()))
                return false;

                    switch (comp.ComponentType)
                    {
                        case ComponentType.PRE_DRAWABLE:
                            _preDrawables.Add(comp);
                            break;
                        case ComponentType.POS_DRAWABLE:
                            _posDrawables.Add(comp);
                            break;
                        case ComponentType.UPDATEABLE:
                            _updateables.Add(comp);
                            break;
                        case ComponentType.PRE_DRAWABLE_AND_UPDATEABLE:
                            _preDrawables.Add(comp);
                            _updateables.Add(comp);
                            break;
                        case ComponentType.POS_DRAWABLE_AND_UPDATEABLE:
                            _posDrawables.Add(comp);
                            _updateables.Add(comp);
                            break;
                        case ComponentType.POS_WITHDEPTH_DRAWABLE:
                            _posWithDepthDrawables.Add(comp);
                            break;
                        case ComponentType.POS_WITHDEPTH_DRAWABLE_AND_UPDATEABLE:
                            _posWithDepthDrawables.Add(comp);
                            _updateables.Add(comp);
                            break;
                        case ComponentType.NONE:
                            break;
                        default:
                            break;
                    }                                
                  _comps.Add(comp.getMyName(), comp);
                  _comps[comp.getMyName()].iInitialize();
                  _comps[comp.getMyName()].iLoadContent(this.GraphicInfo,factory);
                  CommandProcessor.getCommandProcessor().Register(_comps[comp.getMyName()]);
                  return true;
        }


        /// <summary>
        /// Gets the component.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <returns></returns>
        public IComponent GetComponent(String componentName)
        {            
            return _comps[componentName];
        }

        /// <summary>
        /// Removes the component.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>TRUE is the component exist and was remove, false otherwise</returns>
        public bool RemoveComponent(String name)
        {
            if (!HasComponent(name))
                return false;

            switch (_comps[name].ComponentType)
            {
                case ComponentType.PRE_DRAWABLE:
                    _preDrawables.Remove(_comps[name]);
                    break;
                case ComponentType.POS_DRAWABLE:
                    _posDrawables.Remove(_comps[name]);
                    break;
                case ComponentType.UPDATEABLE:
                    _updateables.Remove(_comps[name]);
                    break;
                case ComponentType.PRE_DRAWABLE_AND_UPDATEABLE:
                    _preDrawables.Remove(_comps[name]);
                    _updateables.Remove(_comps[name]);
                    break;
                case ComponentType.POS_DRAWABLE_AND_UPDATEABLE:
                    _posDrawables.Remove(_comps[name]);
                    _updateables.Remove(_comps[name]);
                    break;
                case ComponentType.POS_WITHDEPTH_DRAWABLE:
                    _posWithDepthDrawables.Remove(_comps[name]);
                    break;
                case ComponentType.POS_WITHDEPTH_DRAWABLE_AND_UPDATEABLE:
                    _posWithDepthDrawables.Remove(_comps[name]);
                    _updateables.Remove(_comps[name]);
                    break;
                case ComponentType.NONE:
                    break;
                default:
                    break;
            }                     
            CommandProcessor.getCommandProcessor().UnRegister(_comps[name]);
            _comps.Remove(name);
            return true;
        }

    }
}
