using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yolov5Net.Scorer;
using Yolov5Net.Scorer.Models.Abstract;

namespace VinXiangQi
{
    public class YoloXiangQiModel : YoloModel
    {
        public override int Width { get; set; } = 640;


        public override int Height { get; set; } = 640;


        public override int Depth { get; set; } = 3;


        public override int Dimensions { get; set; } = 20;


        public override int[] Strides { get; set; } = new int[3] { 8, 16, 32 };


        public override int[][][] Anchors { get; set; } = new int[3][][]
        {
            new int[3][]
            {
                new int[2] { 10, 13 },
                new int[2] { 16, 30 },
                new int[2] { 33, 23 }
            },
            new int[3][]
            {
                new int[2] { 30, 61 },
                new int[2] { 62, 45 },
                new int[2] { 59, 119 }
            },
            new int[3][]
            {
                new int[2] { 116, 90 },
                new int[2] { 156, 198 },
                new int[2] { 373, 326 }
            }
        };


        public override int[] Shapes { get; set; } = new int[3] { 80, 40, 20 };


        public override float Confidence { get; set; } = 0.6f;


        public override float MulConfidence { get; set; } = 0.6f;


        public override float Overlap { get; set; } = 0.5f;


        public override string[] Outputs { get; set; } = new string[1] { "output" };


        public override List<YoloLabel> Labels { get; set; } = new List<YoloLabel>
        {
            // ['b_ma', 'b_xiang', 'b_shi', 'b_jiang', 'b_che', 'b_pao', 'b_bing', 'r_che', 'r_ma', 'r_shi', 'r_jiang', 'r_xiang', 'r_pao', 'r_bing', 'board']  # class names
            new YoloLabel
            {
                Id = 1,
                Name = "b_ma"
            },
            new YoloLabel
            {
                Id = 2,
                Name = "b_xiang"
            },
            new YoloLabel
            {
                Id = 3,
                Name = "b_shi"
            },
            new YoloLabel
            {
                Id = 4,
                Name = "b_jiang"
            },
            new YoloLabel
            {
                Id = 5,
                Name = "b_che"
            },
            new YoloLabel
            {
                Id = 6,
                Name = "b_pao"
            },
            new YoloLabel
            {
                Id = 7,
                Name = "b_bing"
            },
            new YoloLabel
            {
                Id = 8,
                Name = "r_che"
            },
            new YoloLabel
            {
                Id = 9,
                Name = "r_ma"
            },
            new YoloLabel
            {
                Id = 10,
                Name = "r_shi"
            },
            new YoloLabel
            {
                Id = 11,
                Name = "r_jiang"
            },
            new YoloLabel
            {
                Id = 12,
                Name = "r_xiang"
            },
            new YoloLabel
            {
                Id = 13,
                Name = "r_pao"
            },
            new YoloLabel
            {
                Id = 14,
                Name = "r_bing"
            },
            new YoloLabel
            {
                Id = 15,
                Name = "board"
            }
        };


        public override bool UseDetect { get; set; } = true;

    }
}
