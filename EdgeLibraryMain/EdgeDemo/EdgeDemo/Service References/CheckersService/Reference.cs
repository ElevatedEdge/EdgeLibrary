﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EdgeDemo.CheckersService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CheckersService.ICheckersService")]
    public interface ICheckersService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/AddMove", ReplyAction="http://tempuri.org/ICheckersService/AddMoveResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[][]))]
        void AddMove(object[] moveInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/GetMovesAfter", ReplyAction="http://tempuri.org/ICheckersService/GetMovesAfterResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[][]))]
        object[][] GetMovesAfter(System.DateTime time);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICheckersServiceChannel : EdgeDemo.CheckersService.ICheckersService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CheckersServiceClient : System.ServiceModel.ClientBase<EdgeDemo.CheckersService.ICheckersService>, EdgeDemo.CheckersService.ICheckersService {
        
        public CheckersServiceClient() {
        }
        
        public CheckersServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CheckersServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CheckersServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CheckersServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void AddMove(object[] moveInfo) {
            base.Channel.AddMove(moveInfo);
        }
        
        public object[][] GetMovesAfter(System.DateTime time) {
            return base.Channel.GetMovesAfter(time);
        }
    }
}
