using System.Threading;

#region Потоки Independent

Thread myThread = new Thread(Print);
myThread.Start();

for (int i = 0; i < 5; i++)
{
    Console.WriteLine($"Главный поток: {i}");
    Thread.Sleep(300);
}

void Print()
{
    for (int i = 0; i < 5; i++)
    {
        Console.WriteLine($"Второй поток: {i}");
        Thread.Sleep(400);
    }
}
Console.WriteLine(new string('-', 25));

#endregion


#region Потоки с параметрами и ParameterizedThreadStart

Thread myThread1 = new Thread(new ParameterizedThreadStart(PrintByText));
Thread myThread2 = new Thread(PrintByText);
Thread myThread3 = new Thread(message => Console.WriteLine(message));

myThread1.Start("Hello");
myThread2.Start("Привет");
myThread3.Start("Salut");

int number = 4;
Thread myThread4 = new Thread(PrintInt);
myThread4.Start(number);    

Person tom = new Person("Tom", 37);
Thread myThread5 = new Thread(PrintPerson);
myThread5.Start(tom);

void PrintPerson(object? obj)
{
    // здесь мы ожидаем получить объект Person
    if (obj is Person person)
    {
        Console.WriteLine($"Name = {person.Name}");
        Console.WriteLine($"Age = {person.Age}");
    }
}


void PrintByText(object? message) => Console.WriteLine(message);

void PrintInt(object? obj)
{
    // здесь мы ожидаем получить число
    if (obj is int n)
    {
        Console.WriteLine($"n * n = {n * n}");
    }
}
Console.WriteLine(new string('-', 25));

#endregion


#region Синхронизация потоков
int x = 0;
object locker = new();  // объект-заглушка
AutoResetEvent waitHandler = new AutoResetEvent(true);  // объект-событие

for (int i = 1; i < 6; i++)
{
    Thread myThread6 = new(PrintAutoResetEvent);
    myThread6.Name = $"Поток {i}";   // устанавливаем имя для каждого потока
    myThread6.Start();
}

void PrintLoop()
{
    lock (locker)
    {
        x = 1;
        for (int i = 1; i < 6; i++)
        {
            Console.WriteLine($"{Thread.CurrentThread.Name}: {x}");
            x++;
            Thread.Sleep(100);
        }
    }
}
Console.WriteLine(new string('-',25));

void PrintLockTypeMonitor()
{
    bool acquiredLock = false;
    try
    {
        Monitor.Enter(locker, ref acquiredLock);
        x = 1;
        for (int i = 1; i < 6; i++)
        {
            Console.WriteLine($"{Thread.CurrentThread.Name}: {x}");
            x++;
            Thread.Sleep(100);
        }
    }
    finally
    {
        if (acquiredLock) Monitor.Exit(locker);
    }
}

void PrintAutoResetEvent()
{
    waitHandler.WaitOne();  // ожидаем сигнала
    x = 1;
    for (int i = 1; i < 6; i++)
    {
        Console.WriteLine($"{Thread.CurrentThread.Name}: {x}");
        x++;
        Thread.Sleep(100);
    }
    waitHandler.Set();  //  сигнализируем, что waitHandler в сигнальном состоянии
}

// запускаем пять потоков
for (int i = 1; i < 6; i++)
{
    Reader reader = new Reader(i);
}
class Reader
{
    // создаем семафор
    static Semaphore sem = new Semaphore(3, 3);
    Thread myThread;
    int count = 3;// счетчик чтения

    public Reader(int i)
    {
        myThread = new Thread(Read);
        myThread.Name = $"Читатель {i}";
        myThread.Start();
    }

    public void Read()
    {
        while (count > 0)
        {
            sem.WaitOne();  // ожидаем, когда освободиться место

            Console.WriteLine($"{Thread.CurrentThread.Name} входит в библиотеку");

            Console.WriteLine($"{Thread.CurrentThread.Name} читает");
            Thread.Sleep(1000);

            Console.WriteLine($"{Thread.CurrentThread.Name} покидает библиотеку");

            sem.Release();  // освобождаем место

            count--;
            Thread.Sleep(1000);
        }
    }
}

#endregion

record class Person(string Name, int Age);
