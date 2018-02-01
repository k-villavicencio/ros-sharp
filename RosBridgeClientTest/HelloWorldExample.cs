using System;
using RosSharp.RosBridgeClient;
using System.Collections.Generic;

namespace RosBridgeClientExamples
{
    // roscore
    // rostopic echo /cmd_vel
    // rostopic pub /listener std_msgs/String "Hello World!"
    // rosrun rospy_tutorials add_two_ints_server
    // roslaunch rosbridge_server rosrbridge_websocket.launch

    public class HelloWorldExample
    {
        public static void Main(string[] args)
        {
            RosSocket rosSocket = new RosSocket("ws://192.168.56.102:9090");
           
            // Publication Example:
            int publication_id = rosSocket.Advertize("/cmd_vel", "geometry_msgs/Twist");

            GeometryTwist geometryMsgsTwist = new GeometryTwist();

            geometryMsgsTwist.linear = new GeometryVector3();
            geometryMsgsTwist.linear.x = 0.1f;
            geometryMsgsTwist.linear.y = 0.2f;
            geometryMsgsTwist.linear.z = 0.3f;

            geometryMsgsTwist.angular = new GeometryVector3();
            geometryMsgsTwist.angular.x = 0.4f;
            geometryMsgsTwist.angular.y = 0.5f;
            geometryMsgsTwist.angular.z = 0.6f;
            rosSocket.Publish(publication_id, geometryMsgsTwist);

            // Subscription Example:
            int subscription_id = rosSocket.Subscribe("/listener", "std_msgs/String", subscriptionHandler);
            int odom_id = rosSocket.Subscribe("/odom", "nav_msgs/Odometry", odomHandler);

            // Service Example:
            AddTwoIntsInput addTwoIntsParameters = new AddTwoIntsInput(19, 23);
            int service_id = rosSocket.CallService("/add_two_ints", typeof(AddTwoIntsOutput), addTwoIntsHandler, addTwoIntsParameters);

            Console.WriteLine("Press any key to close...");
            Console.ReadKey(true);
            rosSocket.Close();
        }

        private static void odomHandler(Message message)
        {
            NavigationOdometry odom = (NavigationOdometry)message;
            Console.WriteLine(
                odom.pose.pose.position.x + " " +
                odom.pose.pose.position.y + " " +
                odom.pose.pose.position.z);
        }

        private static void subscriptionHandler(Message message)
        {
            StandardString standardString = (StandardString)message;
            Console.WriteLine(standardString.data);
        }

        private static void addTwoIntsHandler(object result)
        {
            Console.WriteLine(result.ToString());
        }
    }

    public class AddTwoIntsInput
    {
        public int a;
        public int b;
        public AddTwoIntsInput(int A, int B)
        {
            a = A;
            b = B;
        }
    }
    public class AddTwoIntsOutput
    {
        public int sum;
        public override string ToString()
        {
            return "The answer is " + sum.ToString() +"!";
        }
    }
}