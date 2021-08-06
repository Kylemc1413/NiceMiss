using NiceMiss.UI;
using Zenject;

namespace NiceMiss.Installers
{
    public class NiceMissMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ModifierUI>().AsSingle();
            Container.BindInterfacesAndSelfTo<HitscoreModal>().AsSingle();
        }
    }
}
