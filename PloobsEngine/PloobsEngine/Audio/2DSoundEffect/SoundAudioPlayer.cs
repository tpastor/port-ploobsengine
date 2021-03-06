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
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using PloobsEngine.SceneControl;
using PloobsEngine.Engine.Logger;
using PloobsEngine.Engine;

namespace PloobsEngine.Audio
{
    /// <summary>
    /// SoundPlayer
    /// </summary>
    public class SoundAudioPlayer
    {
        private Dictionary<string, SimpleSoundEffect> musics = new Dictionary<string, SimpleSoundEffect>();
        private GraphicFactory manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundAudioPlayer"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public SoundAudioPlayer(GraphicFactory factory)
        {
            System.Diagnostics.Debug.Assert(factory != null);
            this.manager = factory;
        }

        /// <summary>
        /// Plays the effect song.
        /// </summary>
        /// <param name="songName">Name of the song.</param>
        public void PlaySoundEffect(string songName)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(songName));
            System.Diagnostics.Debug.Assert(musics.ContainsKey(songName));
            musics[songName].Play();
        }

        /// <summary>
        /// Pauses the effect song.
        /// </summary>
        /// <param name="songName">Name of the song.</param>
        public void PauseSoundEffect(string songName)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(songName));
            System.Diagnostics.Debug.Assert(musics.ContainsKey(songName));
            musics[songName].Pause();
        }

        /// <summary>
        /// Resumes the specified song name.
        /// </summary>
        /// <param name="songName">Name of the song.</param>
        public void ResumeSoundEffect(string songName)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(songName));
            System.Diagnostics.Debug.Assert(musics.ContainsKey(songName));
            musics[songName].Resume();
        }

        /// <summary>
        /// Stops the effect song.
        /// </summary>
        /// <param name="songName">Name of the song.</param>
        public void StopSoundEffect(string songName)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(songName));
            System.Diagnostics.Debug.Assert(musics.ContainsKey(songName));
            musics[songName].Stop();
        }

        /// <summary>
        /// State of the parameter songName
        /// </summary>
        /// <param name="songName">Name of the song.</param>
        /// <returns></returns>
        public SoundState SoundState(string songName)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(songName));
            System.Diagnostics.Debug.Assert(musics.ContainsKey(songName));
            return musics[songName].State;
        }


        /// <summary>
        /// Add A Sound Effect to the Repo
        /// Sounds are shared here
        /// </summary>
        /// <param name="musicNamePath">filepath</param>
        /// <param name="musicName">Name used to refer to this effect latter</param>
        /// <param name="volume">between 0 - 1</param>
        /// <param name="pitch">between -1 to 1 (octaves)</param>
        /// <param name="pan">between -1 to 1 (left - right)</param>
        /// <param name="isLooped">if set to <c>true</c> [is looped].</param>
        public void AddSoundToRepository(string musicNamePath, String musicName, float volume = 1, float pitch = 0, float pan = 0, bool isLooped = false)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(musicNamePath));
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(musicName));
            SimpleSoundEffect sse = new SimpleSoundEffect(manager,musicNamePath, volume, pitch, pan, isLooped);
            if (musics.ContainsKey(musicName))
            {
                ActiveLogger.LogMessage("Already contains this song " + musicName + " , Overwriting", LogLevel.Warning);
            }
            musics[musicName] = sse;
        }

        /// <summary>
        /// Removes the effect from repository.
        /// </summary>
        /// <param name="soundEffectName">Name of the sound effect.</param>
        public void RemoveSoundFromRepository(string soundEffectName)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(soundEffectName));
            if (String.IsNullOrEmpty(soundEffectName))
            {
                ActiveLogger.LogMessage("Bad sound effect name (null/empty), not removing the sound", LogLevel.Warning);
                return;
            }

            if (musics.ContainsKey(soundEffectName))
                musics.Remove(soundEffectName);
            else
            {
                ActiveLogger.LogMessage("Sound not found on repo: " + soundEffectName, LogLevel.Warning);
            }
        }


        /// <summary>
        /// Determines whether [This Audio Player has] [the specified sound effect name].
        /// </summary>
        /// <param name="soundEffectName">Name of the sound effect.</param>
        /// <returns>
        ///   <c>true</c> if [has the specified sound effect] otherwise, <c>false</c>.
        /// </returns>
        public bool HasSoundEffect(String soundEffectName)
        {
            if (String.IsNullOrEmpty(soundEffectName))
            {
                ActiveLogger.LogMessage("Bad sound effect name (null/empty) on GetSound(), returning false", LogLevel.Warning);
                return false;
            }

            if (musics.ContainsKey(soundEffectName))
            {
                return true;
            }
            else
            {                
                return false;
            }
        }

        /// <summary>
        /// Gets the sound effect instance.
        /// The SimpleSoundEffect MUST BE ALREADY IN THE REPO !!!
        /// TO CREATE A Non SHARED SIMPLESOUNDEFFECT, USE THE SIMPLESOUNDEFFECT CONSTRUTOR.
        /// </summary>
        /// <param name="soundEffectName">Name of the sound effect.</param>
        /// <returns></returns>
        public SimpleSoundEffect GetSoundEffectInstance(String soundEffectName)
        {
            if (String.IsNullOrEmpty(soundEffectName))
            {
                ActiveLogger.LogMessage("Bad sound effect name (null/empty) on GetSound(), returning null", LogLevel.Warning);
                return null;
            }

            if (musics.ContainsKey(soundEffectName))
            {
                return musics[soundEffectName];
            }
            else
            {
                ActiveLogger.LogMessage("Sound Not Found in this Repo", LogLevel.Warning);
                return null;
            }
        }

        /// <summary>
        /// Removes all sounds.
        /// </summary>
        public void RemoveAllSounds()
        {
            musics.Clear();
        }

        /// <summary>
        /// Cleans up.
        /// Not called by the engine
        /// </summary>
        public void CleanUp()
        {
            foreach (var item in musics.Keys)
            {
                manager.ReleaseAsset(item);
            }            
            musics.Clear();
        }        
    }
}
