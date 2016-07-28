## VisualStudio.Fixie

...

## Setup

- Add fixie convention to test project.

```csharp
public class ClassConvention : Convention {
    public ClassConvention() {
        var target = ConfigurationManager.AppSettings.Get("fixie");
        Classes.Where(x => x.Name == target);
    }
}
```

- Create cake task

```csharp
Task("fixie")
    .Does(() => {
        var config = testDll + ".config";
        var className = Argument("className", "");
        TransformConfig(config, new TransformationCollection {
                { "configuration/appSettings/add[@key='fixie']/@value", className }
        });
        Fixie(testDll);
    });
```

- Execute test from Fixie Menu

>