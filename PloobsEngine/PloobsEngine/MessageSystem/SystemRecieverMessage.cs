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
using PloobsEngine.Entity;


namespace PloobsEngine.MessageSystem
{
    /// <summary>    
    /// Helper to Handle some message
    /// </summary>
    public class SystemRecieverMessage : IRecieveMessageEntity, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemRecieverMessage"/> class.
        /// </summary>
        public SystemRecieverMessage(SenderType AcceptableMessagesTypes = SenderType.SYSTEM)
        {
            this.AcceptableMessagesTypes = AcceptableMessagesTypes;
            EntityMapper.getInstance().AddEntity(this);
        }
        SenderType AcceptableMessagesTypes;
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemRecieverMessage"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public SystemRecieverMessage(long id)
        {
            this.id = id;
            EntityMapper.getInstance().AddEntity(this);
        }
        
        #region IRecieveMessageEntity Members

        /// <summary>
        /// Handles a message from determined sender type.
        /// </summary>
        /// <param name="type">Sender type.</param>
        /// <returns></returns>
        public bool HandleThisMessageType(SenderType type)
        {
            return type == AcceptableMessagesTypes;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="mes">The mes.</param>
        public virtual void HandleMessage(Message mes)
        {
            if (OnMessage != null)
                OnMessage(mes);
        }

        public event Action<Message> OnMessage;

        #endregion

        #region IEntity Members
        long id;
        /// <summary>
        /// return the entity id
        /// </summary>
        /// <returns>
        /// the id
        /// </returns>
        public long GetId()
        {
            return id;
        }

        /// <summary>
        /// sets the id
        /// </summary>
        /// <param name="id"></param>
        public void SetId(long id)
        {
            this.id = id;
        }

        #endregion
    
        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void  Dispose()
        {
            EntityMapper.getInstance().RemoveEntity(this);
        }

        #endregion
}
}
