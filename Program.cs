using GestionCitas;
using GestionCitas_Laboratorio3.Backend;
using Spectre.Console;

public class Program
{
    static void Main(string[] args)
    {
        var usuariosManager = new UsuariosManager();
        var usuarios = usuariosManager.MenuUsuarios();
        var login = new Login(usuarios);

        MostrarMenuPrincipal(usuarios, login);
    }

    static void MostrarMenuPrincipal(List<Usuario> usuarios, Login login)
    {
        Usuario usuario = null;

        while (usuario == null)
        {
            try
            {
                AnsiConsole.MarkupLine("[bold cyan]BIENVENIDO AL SISTEMA DE CITAS MEDICAS[/]");
                AnsiConsole.MarkupLine("[bold cyan]Ingrese su correo:[/]");
                string email = AnsiConsole.Prompt(new TextPrompt<string>("") { PromptStyle = Style.Parse("white") });

                AnsiConsole.MarkupLine("[bold cyan]Ingrese su contraseña:[/]");
                string password = AnsiConsole.Prompt(new TextPrompt<string>("") { PromptStyle = Style.Parse("white") });

                usuario = login.Autenticar(email, password);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"{ex.Message}");
            }
        }

        AnsiConsole.MarkupLine($"Bienvenido [bold cyan]{usuario.NombreCompleto}[/]");

        switch (usuario)
        {
            case Administrador admin:
                Administrador.ShowAdministratorMenu(usuarios.OfType<Doctor>().ToList(), usuarios.OfType<Paciente>().ToList(), usuarios, login);
                break;
            case Doctor doctor:
                MostrarMenuDoctor(doctor, usuarios, login);
                break;
            case Paciente paciente:
                MostrarMenuPaciente(paciente, usuarios.OfType<Doctor>().ToList(), usuarios, login);
                break;
            default:
                AnsiConsole.MarkupLine("[red]Tipo de usuario no reconocido.[/]");
                break;
        }
    

    AnsiConsole.MarkupLine($"Bienvenido [bold cyan]{usuario.NombreCompleto}[/]");

        switch (usuario)
        {
            case Administrador admin:
                MostrarMenuAdministrador(usuarios.OfType<Doctor>().ToList(), usuarios.OfType<Paciente>().ToList(), usuarios, login);
                break;
            case Doctor doctor:
                MostrarMenuDoctor(doctor, usuarios, login);
                break;
            case Paciente paciente:
                MostrarMenuPaciente(paciente, usuarios.OfType<Doctor>().ToList(), usuarios, login);
                break;
            default:
                AnsiConsole.MarkupLine("[red]Tipo de usuario no reconocido.[/]");
                break;
        }
    }








     public static void MostrarMenuAdministrador(List<Doctor> doctores, List<Paciente> pacientes, List<Usuario> usuarios, Login login)
    {
        bool salir = false;
        while (!salir)
        {
            var opciones = new[] { "Ver listado de doctores", "Ver listado de pacientes", "Salir" };
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold cyan]Menú del Administrador[/]")
                    .AddChoices(opciones)
            );

            switch (choice)
            {
                case "Ver listado de doctores":
                    Administrador.ShowDoctors(doctores);
                    break;
                case "Ver listado de pacientes":
                    Administrador.ShowPatients(pacientes);
                    break;
                case "Salir":
                    salir = true;
                    MostrarMenuPrincipal(usuarios, login);
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Opción no válida.[/]");
                    break;
            }
        }
    }

    public static void MostrarMenuDoctor(Doctor doctor, List<Usuario> usuarios, Login login)
    {
        bool salir = false;
        while (!salir)
        {
            var opciones = new[] { "Ver citas programadas", "Ver historial de un paciente", "Salir" };
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold cyan]Menú del Doctor[/]")
                    .AddChoices(opciones)
            );

            switch (choice)
            {
                case "Ver citas programadas":
                    doctor.VerCitas();
                    break;
                case "Ver historial de un paciente":
                    AnsiConsole.MarkupLine("[bold yellow]Seleccione el paciente:[/]");
                    var pacienteChoice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .AddChoices(doctor.Citas.Select(c => c.Paciente.NombreCompleto))
                    );
                    var pacienteSeleccionado = doctor.Citas.Select(c => c.Paciente).FirstOrDefault(p => p.NombreCompleto == pacienteChoice);
                    if (pacienteSeleccionado != null)
                    {
                        doctor.VerHistorialPaciente(pacienteSeleccionado);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Paciente no encontrado.[/]");
                    }
                    break;
                case "Salir":
                    salir = true;
                    MostrarMenuPrincipal(usuarios, login);
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Opción no válida.[/]");
                    break;
            }
        }
    }

   public  static void MostrarMenuPaciente(Paciente paciente, List<Doctor> doctores, List<Usuario> usuarios, Login login)
    {
        bool salir = false;
        while (!salir)
        {
            var opciones = new[] { "Ver historial médico", "Asignar cita con un doctor", "Salir" };
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold cyan]Menú del Paciente[/]")
                    .AddChoices(opciones)
            );

            switch (choice)
            {
                case "Ver historial médico":
                    paciente.VerHistorial();
                    break;
                case "Asignar cita con un doctor":
                    AnsiConsole.MarkupLine("[bold yellow]Seleccione el doctor:[/]");
                    var doctorChoice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .AddChoices(doctores.Select(d => d.NombreCompleto))
                    );
                    var doctorSeleccionado = doctores.FirstOrDefault(d => d.NombreCompleto == doctorChoice);
                    if (doctorSeleccionado != null)
                    {
                        AnsiConsole.MarkupLine("[bold yellow]Ingrese la fecha de la cita (formato: yyyy-MM-dd):[/]");
                        DateTime fecha = AnsiConsole.Prompt(new TextPrompt<DateTime>("") { PromptStyle = Style.Parse("white") });

                        AnsiConsole.MarkupLine("[bold yellow]Ingrese notas para la cita:[/]");
                        string notas = AnsiConsole.Prompt(new TextPrompt<string>("") { PromptStyle = Style.Parse("white") });

                        paciente.AsignarCita(doctorSeleccionado, fecha, notas);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Doctor no encontrado.[/]");
                    }
                    break;
                case "Salir":
                    salir = true;
                    MostrarMenuPrincipal(usuarios, login);
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Opción no válida.[/]");
                    break;
            }
        }
    }
}
