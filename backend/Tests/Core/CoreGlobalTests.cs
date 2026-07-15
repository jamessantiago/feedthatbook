using Core;

namespace Tests.Core;

public class CoreGlobalTests
{
    [Test]
    public void Settings_DefaultIsNotNull()
    {
        Assert.That(CoreGlobal.Settings, Is.Not.Null);
    }

    [Test]
    public void Settings_CanSetAndGet()
    {
        var original = CoreGlobal.Settings;
        var custom = new Settings { GeminiApiKey = "test-key", GeminiModelId = "test-model" };
        CoreGlobal.Settings = custom;
        Assert.That(CoreGlobal.Settings, Is.SameAs(custom));
        CoreGlobal.Settings = original;
    }

    [Test]
    public void Settings_HasDefaultValues()
    {
        var s = new Settings();
        Assert.Multiple(() =>
        {
            Assert.That(s.GeminiApiKey, Is.Null);
            Assert.That(s.GeminiModelId, Is.EqualTo("gemini-flash-latest"));
        });
    }
}
