using Xunit;

namespace Kros.Utils.UnitTests
{
    public abstract class DiContainerTester<T> where T : IDiContainer, new()
    {
        #region Helpers

        protected interface IFoo { }
        protected interface IBar { }
        protected class Foo : IFoo { }
        protected class FooChild : IFoo { }
        protected class Bar : IBar
        {
            public Bar(IFoo foo)
            {
                Foo = foo;
            }

            public IFoo Foo { get; }
        }

        protected IDiContainer CreateConntainer()
        {
            return new T();
        }

        #endregion

        #region Tests

        [Fact]
        public void RegisterClassTypeAndReturnNewInstanceOnEachResolve()
        {
            IDiContainer container = CreateConntainer();

            container.Register<Foo>();

            Foo instance1 = container.GetInstance<Foo>();
            Assert.NotNull(instance1);

            Foo instance2 = container.GetInstance<Foo>();
            Assert.NotSame(instance1, instance2);
        }

        [Fact]
        public void RegisterNamedClassTypeAndReturnNewInstanceOnEachResolve()
        {
            IDiContainer container = CreateConntainer();

            container
                .Register<Foo>()
                .Register<Foo>("foo2");

            Foo instance1 = container.GetInstance<Foo>();
            Assert.NotNull(instance1);

            Foo instance2 = container.GetInstance<Foo>("foo2");
            Assert.NotSame(instance1, instance2);
        }

        [Fact]
        public void RegisterInterfaceWithClassAndReturnNewInstanceOnEachResolve()
        {
            IDiContainer container = CreateConntainer();

            container.Register<IFoo, Foo>();

            IFoo instance1 = container.GetInstance<IFoo>();
            Assert.IsType<Foo>(instance1);

            IFoo instance2 = container.GetInstance<IFoo>();
            Assert.IsType<Foo>(instance2);
            Assert.NotSame(instance1, instance2);
        }

        [Fact]
        public void RegisterNamedInterfaceWithClassAndReturnNewInstanceOnEachResolve()
        {
            IDiContainer container = CreateConntainer();

            container
                .Register<IFoo, Foo>()
                .Register<IFoo, Foo>("foo2");

            IFoo instance1 = container.GetInstance<IFoo>();
            Assert.IsType<Foo>(instance1);

            IFoo instance2 = container.GetInstance<IFoo>("foo2");
            Assert.IsType<Foo>(instance2);
            Assert.NotSame(instance1, instance2);
        }

        [Fact]
        public void RegisterClassTypeUsingLambdaAndReturnNewInstanceOnEachResolve()
        {
            IDiContainer container = CreateConntainer();

            container.Register<IFoo>(c => new Foo());

            IFoo instance1 = container.GetInstance<IFoo>();
            Assert.IsType<Foo>(instance1);

            IFoo instance2 = container.GetInstance<IFoo>();
            Assert.IsType<Foo>(instance2);
            Assert.NotSame(instance1, instance2);
        }

        [Fact]
        public void RegisterNamedClassTypeUsingLambdaAndReturnNewInstanceOnEachResolve()
        {
            IDiContainer container = CreateConntainer();

            container
                .Register<IFoo>(c => new Foo())
                .Register<IFoo>("foo2", c => new Foo());

            IFoo instance1 = container.GetInstance<IFoo>();
            Assert.IsType<Foo>(instance1);

            IFoo instance2 = container.GetInstance<IFoo>("foo2");
            Assert.IsType<Foo>(instance2);
            Assert.NotSame(instance1, instance2);
        }

        [Fact]
        public void RegisterClassTypeAsSingleton()
        {
            IDiContainer container = CreateConntainer();

            container.RegisterInstance<Foo>();

            Foo instance1 = container.GetInstance<Foo>();
            Assert.NotNull(instance1);

            Foo instance2 = container.GetInstance<Foo>();
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void RegisterNamedClassTypeAsSingleton()
        {
            IDiContainer container = CreateConntainer();

            container
                .RegisterInstance<Foo>()
                .RegisterInstance<Foo>("foo2");

            Foo instance1 = container.GetInstance<Foo>();
            Assert.NotNull(instance1);

            Foo instance2 = container.GetInstance<Foo>();
            Assert.Same(instance1, instance2);

            Foo instance3 = container.GetInstance<Foo>("foo2");
            Assert.NotSame(instance2, instance3);

            Foo instance4 = container.GetInstance<Foo>("foo2");
            Assert.Same(instance3, instance4);
        }

        [Fact]
        public void RegisterSpecificInstanceAsSingleton()
        {
            Foo specificInstance = new Foo();
            IDiContainer container = CreateConntainer();

            container.RegisterInstance<IFoo>(specificInstance);

            IFoo instance1 = container.GetInstance<IFoo>();
            Assert.Same(specificInstance, instance1);

            IFoo instance2 = container.GetInstance<IFoo>();
            Assert.Same(specificInstance, instance2);
        }

        [Fact]
        public void RegisterNamedSpecificInstanceAsSingleton()
        {
            Foo specificInstance1 = new Foo();
            Foo specificInstance2 = new Foo();
            IDiContainer container = CreateConntainer();

            container
                .RegisterInstance<IFoo>(specificInstance1)
                .RegisterInstance<IFoo>("foo2", specificInstance2);

            IFoo instance1 = container.GetInstance<IFoo>();
            Assert.Same(specificInstance1, instance1);

            IFoo instance2 = container.GetInstance<IFoo>();
            Assert.Same(specificInstance1, instance2);

            IFoo instance3 = container.GetInstance<IFoo>("foo2");
            Assert.Same(specificInstance2, instance3);

            IFoo instance4 = container.GetInstance<IFoo>("foo2");
            Assert.Same(specificInstance2, instance4);
        }

        [Fact]
        public void RegisterInterfaceWithClassAsSingleton()
        {
            IDiContainer container = CreateConntainer();

            container.RegisterInstance<IFoo, Foo>();

            IFoo instance1 = container.GetInstance<IFoo>();
            Assert.NotNull(instance1);

            IFoo instance2 = container.GetInstance<IFoo>();
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void RegisterNamedInterfaceWithClassAsSingleton()
        {
            IDiContainer container = CreateConntainer();

            container
                .RegisterInstance<IFoo, Foo>()
                .RegisterInstance<IFoo, Foo>("foo2");

            IFoo instance1 = container.GetInstance<IFoo>();
            Assert.NotNull(instance1);

            IFoo instance2 = container.GetInstance<IFoo>();
            Assert.Same(instance1, instance2);

            IFoo instance3 = container.GetInstance<IFoo>("foo2");
            Assert.NotNull(instance3);
            Assert.NotSame(instance1, instance3);

            IFoo instance4 = container.GetInstance<IFoo>("foo2");
            Assert.Same(instance3, instance4);
        }

        [Fact]
        public void RegisterInterfaceWithClassUsingLambdaAsSingleton()
        {
            IDiContainer container = CreateConntainer();

            container.RegisterInstance<IFoo>(c => new Foo());

            IFoo instance1 = container.GetInstance<IFoo>();
            Assert.IsType<Foo>(instance1);

            IFoo instance2 = container.GetInstance<IFoo>();
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void RegisterNamedInterfaceWithClassUsingLambdaAsSingleton()
        {
            IDiContainer container = CreateConntainer();

            container
                .RegisterInstance<IFoo>(c => new Foo())
                .RegisterInstance<IFoo>("foo2", c => new Foo());

            IFoo instance1 = container.GetInstance<IFoo>();
            Assert.IsType<Foo>(instance1);

            IFoo instance2 = container.GetInstance<IFoo>();
            Assert.Same(instance1, instance2);

            IFoo instance3 = container.GetInstance<IFoo>("foo2");
            Assert.IsType<Foo>(instance3);
            Assert.NotSame(instance1, instance3);

            IFoo instance4 = container.GetInstance<IFoo>("foo2");
            Assert.Same(instance3, instance4);
        }

        [Fact]
        public void ResolveConstructorDependencies()
        {
            IDiContainer container = CreateConntainer();

            container
                .Register<IFoo, Foo>()
                .Register<IBar, Bar>();

            IBar instance = container.GetInstance<IBar>();
            Bar bar = Assert.IsType<Bar>(instance);
            Assert.IsType<Foo>(bar.Foo);
        }

        [Fact]
        public void ResolveItemsFormChildContainer()
        {
            IDiContainer container = CreateConntainer();

            container
                .Register<IFoo, Foo>()
                .Register<IBar, Bar>();

            IDiContainer childContainer = container.CreateChildContainer();
            childContainer.Register<IFoo, FooChild>();

            IFoo instance = container.GetInstance<IFoo>();
            Assert.IsType<Foo>(instance);

            IFoo childInstance = childContainer.GetInstance<IFoo>();
            Assert.IsType<FooChild>(childInstance);

            IBar parentInstance = childContainer.GetInstance<IBar>();
            Bar parentBar = Assert.IsType<Bar>(parentInstance);
            Assert.IsType<FooChild>(parentBar.Foo);
        }

        #endregion
    }
}
