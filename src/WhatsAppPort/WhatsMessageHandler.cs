﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhatsAppApi.Account;
using WhatsAppApi.Parser;
using WhatsAppApi.Response;

namespace WhatsAppPort
{
    class WhatsMessageHandler
    {
        private Dictionary<string, List<FMessage>> messageHistory;

        private Dictionary<string, WhatsEventHandler.MessageRecievedHandler> userMessageDict; 

        public WhatsMessageHandler()
        {
            this.messageHistory = new Dictionary<string, List<FMessage>>();
            this.userMessageDict = new Dictionary<string, WhatsEventHandler.MessageRecievedHandler>();
            WhatsEventHandler.MessageRecievedEvent += WhatsEventHandlerOnMessageRecievedEvent;
        }

        private void WhatsEventHandlerOnMessageRecievedEvent(FMessage mess)
        {
            if (mess == null || mess.key.remote_jid == null || mess.key.remote_jid.Length == 0)
                return;

            if(!this.messageHistory.ContainsKey(mess.key.remote_jid))
                this.messageHistory.Add(mess.key.remote_jid, new List<FMessage>());

            this.messageHistory[mess.key.remote_jid].Add(mess);
            this.CheckIfUserRegisteredAndCreate(mess);
        }

        private void CheckIfUserRegisteredAndCreate(FMessage mess)
        {
            if (this.messageHistory.ContainsKey(mess.key.remote_jid))
                return;

            var jidSplit = mess.key.remote_jid.Split('@');
            WhatsContact tmpWhatsUser = new WhatsContact(jidSplit[0], jidSplit[1], mess.key.serverNickname);
            User tmpUser = new User(jidSplit[0], jidSplit[1]);
            tmpUser.SetUser(tmpWhatsUser);

            this.messageHistory.Add(mess.key.remote_jid, new List<FMessage>());
            this.messageHistory[mess.key.remote_jid].Add(mess);
        }

        public void RegisterUser(User user, WhatsEventHandler.MessageRecievedHandler messHandler)
        {
            
        }
    }
}
