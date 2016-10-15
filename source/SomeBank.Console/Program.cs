using Topshelf;

namespace SomeBank.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>                                
            {
                x.Service<SomeBankService>(s =>                       
                {
                    s.ConstructUsing(name => new SomeBankService());  
                    s.WhenStarted(tc => tc.Start());            
                    s.WhenStopped(tc => tc.Stop());             
                });
                x.RunAsLocalSystem();                           

                x.SetDescription("SomeBank Domain Service");       
                x.SetDisplayName("SomeBankDomainService");                      
                x.SetServiceName("SomeBankDomainService");                      
            });                                                         
        }

    }
}