using Zenject;

namespace NiceMiss.Installers
{
    public class NiceMissGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<NiceMissManager>().AsSingle();
            if (Config.enabled)
            {
                Container.BindInterfacesAndSelfTo<NoteTracker>().AsSingle();
                Container.BindInterfacesAndSelfTo<NoteOutliner>().AsSingle();
            }
        }
    }
}
