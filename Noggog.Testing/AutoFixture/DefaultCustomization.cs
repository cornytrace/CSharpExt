﻿using AutoFixture;

namespace Noggog.Testing.AutoFixture
{
    public class DefaultCustomization : ICustomization
    {
        private readonly bool _useMockFileSystem;

        public DefaultCustomization(bool useMockFileSystem = false)
        {
            _useMockFileSystem = useMockFileSystem;
        }
        
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new FileSystemBuilder(_useMockFileSystem));
            fixture.Customizations.Add(new SchedulerBuilder());
            fixture.Customizations.Add(new PathBuilder());
            fixture.Customizations.Add(new CancellationBuilder());
            fixture.Customizations.Add(new ErrorResponseBuilder());
            fixture.Customizations.Add(new ErrorResponseParameterBuilder());
            fixture.Customizations.Add(new GetResponseBuilder());
            fixture.Customizations.Add(new GetResponseParameterBuilder());
            fixture.Customizations.Add(new ProcessBuilder());
            fixture.Behaviors.Add(new ObservableEmptyBehavior());
        }
    }
}