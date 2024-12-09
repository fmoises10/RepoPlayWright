using Microsoft.Playwright;
using System.Text.RegularExpressions;
using Microsoft.Playwright.NUnit;
using System.IO;
//linea diferente para el pull
namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    // Configuración antes de cada prueba
    [SetUp]
    public async Task Setup()
    {
        await Context.Tracing.StartAsync(new()
        {
            Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    // Limpieza después de cada prueba
    [TearDown]
    public async Task TearDown()
    {
        var traceDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "playwright-traces");
        Directory.CreateDirectory(traceDirectory);

        await Context.Tracing.StopAsync(new()
        {
            Path = Path.Combine(traceDirectory, $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip")
        });
    }

    // Test que interactúa con el botón "Get started"
    [Test]
    public async Task InteractWithButton()
    {
        // Navegar al sitio de Playwright y esperar que la red esté inactiva
        await Page.GotoAsync("https://playwright.dev", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Buscar el enlace "Get started" y hacer clic en él
        var getStartedButton = Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });
        await Expect(getStartedButton).ToBeVisibleAsync();
        await getStartedButton.ClickAsync();

        // Verificar que el encabezado de la nueva página con el nombre "Installation" sea visible
        var installationHeading = Page.GetByRole(AriaRole.Heading, new() { Name = "Installation" });
        await Expect(installationHeading).ToBeVisibleAsync();

        // Opcional: Interactuar con otro botón o enlace, por ejemplo, "Command line tools"
        var commandLineToolsButton = Page.GetByRole(AriaRole.Link, new() { Name = "Command line tools" });
        await Expect(commandLineToolsButton).ToBeVisibleAsync();
        await commandLineToolsButton.ClickAsync();

        // Validar que la URL cambió correctamente
        Assert.That(Page.Url, Does.Contain("cli"));
    }
}
