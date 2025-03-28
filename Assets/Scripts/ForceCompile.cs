using UnityEditor;
using UnityEditor.Compilation;

public class ForceCompile
{
    [MenuItem("Tools/Force Compile")]
    static void Compile()
    {
        CompilationPipeline.RequestScriptCompilation();
    }
}
