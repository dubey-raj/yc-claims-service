# Define ALB Target Group
# WARNING: Lifecyle and name_prefix added for testing. Issue discussed here https://github.com/hashicorp/terraform-provider-aws/issues/16889
resource "aws_lb_target_group" "alb-target-group" {
  name_prefix                   = "tg-"
  port                          = 80
  protocol                      = "HTTP"
  target_type                   = "ip"
  vpc_id                        = data.aws_vpc.selected.id
  load_balancing_algorithm_type = "round_robin"

  health_check {
    path    = "/claim-service/healthz"
    matcher = "200"
  }

  stickiness {
    enabled         = true
    type            = "lb_cookie"
    cookie_duration = 86400
  }

  lifecycle {
    create_before_destroy = true
  }


  tags = {
    Name  = format("%s-%s-%s-%s", "albtg", var.Application, var.EnvCode, var.Region)
    rtype = "network"
  }
}

resource "aws_alb_listener_rule" "alb_listener_rule" {
  listener_arn = data.aws_lb_listener.port80_listener.arn
  priority = 10
  action {
    type = "forward"
    target_group_arn = aws_lb_target_group.alb-target-group.arn
  }
  condition {
    path_pattern {
      values = ["/claim-service/*"]
    }
  }
  tags = {
    name  = format("%s-%s-%s-%s", "alb-lr", var.Application, var.EnvCode, var.Region)
    rtype = "network"
  }
}