# Use Amazon S3 for Terraform backend
terraform {
  backend "s3" {}
}

data "aws_vpc" "selected" {
  id = var.vpc_id
}

data "aws_subnets" "public" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.selected.id]
  }
  tags = {
    subtype = "public"
  }
}

data "aws_subnets" "private" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.selected.id]
  }
  tags = {
    subtype = "private"
  }
}

variable "security_group_id" {
  type = string
  description = "Id of existing security group to use for container"
  default = "sg-01ad930b2238b6672"
}

data "aws_security_group" "app01" {
  id = var.security_group_id
}

variable "alb_arn" {
  type = string
  description = "arn of shared load balancer"
  default = "arn:aws:elasticloadbalancing:eu-north-1:934076056444:loadbalancer/app/app-alb-eu-north-1/efb682542b6315fc"
}

data "aws_lb" "alb"{
  arn = var.alb_arn
}

data "aws_lb_listener" "port80_listener" {
  load_balancer_arn = data.aws_lb.alb.arn
  port              = 80
}

locals {
  pvt_subnet_ids_string = join(",", data.aws_subnets.private.ids)
  pvt_subnet_ids_list = split(",", local.pvt_subnet_ids_string)
  pub_subnet_ids_string = join(",", data.aws_subnets.public.ids)
  pub_subnet_ids_list = split(",", local.pub_subnet_ids_string)
}
