#tool "nuget:?package=Fixie"
#addin "nuget:?package=Cake.Watch"
#addin "MagicChunks"

var user = EnvironmentVariable("ghu");
var pass = EnvironmentVariable("ghp");

var solution = "VisualStudio.Fixie.sln";
var testProj = "VisualStudio.Fixie.Tests/VisualStudio.Fixie.Tests.csproj";
var testDll = "VisualStudio.Fixie.Tests/bin/Debug/VisualStudio.Fixie.Tests.dll";
var package = "VisualStudio.Fixie/bin/Debug/VisualStudio.Fixie.vsix";
var assemblyInfo = "Visualstudio.Fixie/Properties/AssemblyInfo.cs";

Action<string, string> build = (proj, config) => {
    MSBuild(proj, new MSBuildSettings {
        Verbosity = Verbosity.Minimal,
        ToolVersion = MSBuildToolVersion.VS2015,
        Configuration = config,
        ToolPath = @"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe",
        PlatformTarget = PlatformTarget.MSIL
    });
};

Task("fixie")
    .Does(() => {
        var config = testDll + ".config";
        var className = Argument("className", "");
        TransformConfig(config, new TransformationCollection {
                { "configuration/appSettings/add[@key='fixie']/@value", className }
        });
        Fixie(testDll);
    });

Task("build")
    .Does(() => {
        build(solution, "Debug");
    });

Task("test")
    .Does(() => {
        build(testProj, "Debug");
        Fixie(testDll);
    });

Task("create-github-release")
    .IsDependentOn("build")
    .Does(() => {
        var asm = ParseAssemblyInfo(assemblyInfo);
        var version = asm.AssemblyVersion;
        var tag = string.Format("v{0}", version);
        var args = string.Format("tag -a {0} -m \"{0}\"", tag);
        var owner = "wk-j";
        var repo = "visual-studio-fixie";

        StartProcess("git", new ProcessSettings {
            Arguments = args
        });

        StartProcess("git", new ProcessSettings {
            Arguments = string.Format("push https://{0}:{1}@github.com/wk-j/{2}.git {3}", user, pass, repo, tag)
        });

        GitReleaseManagerCreate(user, pass, owner , repo, new GitReleaseManagerCreateSettings {
            Name              = tag,
            InputFilePath = "RELEASE.md",
            Prerelease        = false,
            TargetCommitish   = "master",
        });
        GitReleaseManagerAddAssets(user, pass, owner, repo, tag, package);
        GitReleaseManagerPublish(user, pass, owner , repo, tag);
    });

var target = Argument("target", "default");
RunTarget(target);