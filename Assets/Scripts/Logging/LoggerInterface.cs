using System.Collections;
public interface Loggable
{
    public string LogObject();

    public string LogFunction(string functionName, string status, string output,params (string, string)[] Parameters);

}