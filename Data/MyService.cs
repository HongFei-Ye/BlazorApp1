using Hangfire;

namespace BlazorApp1.Data
{
    public class MyService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public MyService(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        // 即发即弃作业只执行一次，几乎在创建后立即执行。
        public void ScheduleBackgroundEnqueueJob()
        {
            // Enqueue 方法用于添加一个后台任务
            _backgroundJobClient.Enqueue(() => Console.WriteLine("This is a background Enqueue job."));
        }

        public void ScheduleBackgroundScheduleJob()
        {
            // Enqueue 方法用于添加一个后台任务
            var jobId1 = _backgroundJobClient.Schedule(() => Console.WriteLine("This is a background Schedule job - 1."), TimeSpan.FromDays(7));

            var jobId2 = BackgroundJob.Schedule(() => Console.WriteLine("This is a background Schedule job - 2."),TimeSpan.FromDays(7));

        }


        public void ScheduleBackgroundJobWithData()
        {
            // 可以传递数据给后台任务
            string data = "some data";
            _backgroundJobClient.Enqueue(() => ProcessData(data));
        }

        private void ProcessData(string data)
        {
            // 在后台任务中处理数据
            Console.WriteLine($"Processing data: {data}");
        }

        public void ScheduleRecurringminutesJob()
        {
            // 指定一个唯一的 recurringJobId，以及其他的重复作业选项
            string recurringJobId = "MyRecurringminutesJob"; // 重复作业的唯一标识符
            RecurringJob.AddOrUpdate(recurringJobId, () => Console.WriteLine("This is a recurring job executed every minute"), Cron.Minutely);
        }

        public void ScheduleRecurringDailysJob()
        {
            // 设置重复作业的选项，包括时区设置
            var options = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local // 设置本地时区
            };

            // 指定一个唯一的 recurringJobId，以及其他的重复作业选项
            string recurringJobId = "MyRecurringDailysJob"; // 重复作业的唯一标识符
            RecurringJob.AddOrUpdate(recurringJobId, () => Console.WriteLine("This is a recurring job executed every Daily"), Cron.Daily, options);
        }

        public void ScheduleDailyAM2Task()
        {
            // 设置重复作业的选项，包括时区设置
            var options = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local // 设置本地时区
            };

            // 指定一个唯一的 jobId
            string jobId = "MyDailyTask"; // 任务的唯一标识符

            // 使用 Cron 表达式指定每天早上 2 点执行任务
            string cronExpression = "0 2 * * *"; // 每天早上 2 点执行任务

            // 添加或更新定时任务
            RecurringJob.AddOrUpdate(jobId, () => Console.WriteLine("This is a daily task executed at 2:00 AM"), cronExpression, options);
        }


        public void ScheduleRecurringsecondsJob_1()
        {
            // 指定一个唯一的 recurringJobId，以及其他的重复作业选项
            string recurringJobId = "MyRecurringsecondsJob_1"; // 重复作业的唯一标识符
            RecurringJob.AddOrUpdate(recurringJobId, () => Console.WriteLine("This is a recurring job executed every 20 seconds"), "*/20 * * * * *");
        }

        public void ScheduleRecurringsecondsJob_2()
        {
            // 指定一个唯一的 recurringJobId
            string recurringJobId = "MyRecurringsecondsJob_2"; // 重复作业的唯一标识符

            // 设置重复作业的选项，包括时区设置
            var options = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local // 设置本地时区
            };

            // 使用新的 AddOrUpdate 方法，并传递选项参数
            RecurringJob.AddOrUpdate(recurringJobId, () => Console.WriteLine("This is a recurring job executed every 30 seconds"), "*/30 * * * * *", options);
        }


    }

}
