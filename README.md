# FriendlyCustomSerializerSample
Friendlyでカスタマイズされたシリアライザを利用するときのサンプルです。
EnableUnsafeBinaryFormatterSerializationをfalseにしたアプリをFriedlyで操作したい場合に利用します。
背景としてFriendlyではプロセス間通信にBinaryFormatterを利用しているのでそれが使えなくなります。

以下に利用方法を書きます。
サンプルコードもリポジトリ内にありますので合わせてご確認ください。

## 対応したFriendly
2024/10/09 時点では NuGet Gallery にはシリアライズのカスタムに対応したFriendlyシリーズはアップロードしていません。
本リポジトリ内の [/Nupkgs](Nupkgs)ディレクトリにあるものを使ってください。

## 利用方法
```csharp
WindowsAppFriend.SetCustomSerializer<CustomSerializer>();
``` 
を呼び出すことでシリアライザを登録できます。
注意点としては登録したシリアライザは操作対象プロセスでも使われるため定義しているアセンブリをdllインジェクションします。
操作対象プロセスで動作することを前提としたアセンブリで実装してください。

```csharp
public void Test()
{
    //カスタムシリアライザの設定
    //最初に一回呼び出す
    WindowsAppFriend.SetCustomSerializer<CustomSerializer>();

    //WindowsAppFriendを生成
    var app = new WindowsAppFriend(process);

    //通常のFriendlyの操作
    var formControls = app.AttachFormControls();
    formControls.button.EmulateClick();
    formControls.checkBox.EmulateCheck(CheckState.Checked);
    formControls.comboBox.EmulateChangeText("Item-3");
    formControls.comboBox.EmulateChangeSelect(2);
    formControls.radioButton1.EmulateCheck();
    formControls.radioButton1.EmulateCheck();
}
```

## カスタムシリアライザ
ICustomSerializerを実装します。
実装方法は任意ですが、本サンプルでは[MessagePack](https://www.nuget.org/packages/MessagePack)を利用しています。
### 注意点
シリアライズの可否はMessagePackSerializerに従いますので、BinaryFormatterでシリアライズできていたものでもMessagePackSerializerではシリアライズできないものもあります。
例えばデフォルトコンストラクタが存在しないなどです。その場合はデータを修正してください。

```csharp
public class IntPtrFormatter : IMessagePackFormatter<IntPtr>
{
    public void Serialize(ref MessagePackWriter writer, IntPtr value, MessagePackSerializerOptions options)
        => writer.Write(value.ToInt64());

    public IntPtr Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        => new IntPtr(reader.ReadInt64());
}

public class CustomSerializer : ICustomSerializer
{
    MessagePackSerializerOptions customOptions = MessagePackSerializerOptions
        .Standard
        .WithResolver(
            CompositeResolver.Create(
                new IMessagePackFormatter[] { new IntPtrFormatter() },
                new IFormatterResolver[] { TypelessContractlessStandardResolver.Instance }
            )
        );

    public object Deserialize(byte[] bin)
        => MessagePackSerializer.Typeless.Deserialize(bin, customOptions);

    public Assembly[] GetRequiredAssemblies() => [GetType().Assembly, typeof(MessagePackSerializer).Assembly];

    public byte[] Serialize(object obj)
        => MessagePackSerializer.Typeless.Serialize(obj, customOptions);
}
```
