// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/mytest.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace YY.MicroService.gRPCService {
  public static partial class MyTest
  {
    static readonly string __ServiceName = "MyTest.MyTest";

    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    static readonly grpc::Marshaller<global::YY.MicroService.gRPCService.MyTestRequest> __Marshaller_MyTest_MyTestRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::YY.MicroService.gRPCService.MyTestRequest.Parser));
    static readonly grpc::Marshaller<global::YY.MicroService.gRPCService.MyTestReply> __Marshaller_MyTest_MyTestReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::YY.MicroService.gRPCService.MyTestReply.Parser));

    static readonly grpc::Method<global::YY.MicroService.gRPCService.MyTestRequest, global::YY.MicroService.gRPCService.MyTestReply> __Method_FindLesson = new grpc::Method<global::YY.MicroService.gRPCService.MyTestRequest, global::YY.MicroService.gRPCService.MyTestReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "FindLesson",
        __Marshaller_MyTest_MyTestRequest,
        __Marshaller_MyTest_MyTestReply);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::YY.MicroService.gRPCService.MytestReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of MyTest</summary>
    [grpc::BindServiceMethod(typeof(MyTest), "BindService")]
    public abstract partial class MyTestBase
    {
      public virtual global::System.Threading.Tasks.Task<global::YY.MicroService.gRPCService.MyTestReply> FindLesson(global::YY.MicroService.gRPCService.MyTestRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(MyTestBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_FindLesson, serviceImpl.FindLesson).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, MyTestBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_FindLesson, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::YY.MicroService.gRPCService.MyTestRequest, global::YY.MicroService.gRPCService.MyTestReply>(serviceImpl.FindLesson));
    }

  }
}
#endregion
