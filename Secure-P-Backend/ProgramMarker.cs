namespace Secure_P_Backend;

/// <summary>
/// A marker class used for assembly scanning and dependency injection in the Secure-P Backend application. This class serves as a reference point for identifying the assembly that contains the application's services, controllers, and other components. By using this marker class, you can easily register services and configure dependency injection without hardcoding assembly names, allowing for more maintainable and flexible code. The ProgramMarker class does not contain any logic or properties; its sole purpose is to act as a marker for the application's assembly during service registration and configuration processes.
/// </summary>
public class ProgramMarker
{

}