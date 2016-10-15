using Topshelf;

namespace akkatest
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>                                
            {
                x.Service<BankService>(s =>                       
                {
                    s.ConstructUsing(name => new BankService());  
                    s.WhenStarted(tc => tc.Start());            
                    s.WhenStopped(tc => tc.Stop());             
                });
                x.RunAsLocalSystem();                           

                x.SetDescription("SomeBank Domain Windows Service");       
                x.SetDisplayName("SomeBankDomainService");                      
                x.SetServiceName("SomeBankDomainService");                      
            });                                                         
        }

    }
}