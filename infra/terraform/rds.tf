
resource "aws_db_subnet_group" "midaz" {
  name       = "${var.cluster_name}-db-subnet-group"
  subnet_ids = module.vpc.private_subnets
}

resource "aws_security_group" "midaz_db" {
  name        = "${var.cluster_name}-db-sg"
  description = "DB SG"
  vpc_id      = module.vpc.vpc_id
  ingress {
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    security_groups = [module.eks.node_security_group_id]
  }
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "random_password" "midaz_db_password" {
  length  = 16
  special = true
}

resource "aws_db_instance" "midaz" {
  identifier         = "${var.cluster_name}-pg"
  engine             = "postgres"
  engine_version     = "16.1"
  instance_class     = "db.t3.micro"
  allocated_storage  = 20
  name               = "midaz"
  username           = "midaz"
  password           = random_password.midaz_db_password.result
  db_subnet_group_name = aws_db_subnet_group.midaz.name
  vpc_security_group_ids = [aws_security_group.midaz_db.id]
  skip_final_snapshot = true
}
output "db_endpoint" {
  value = aws_db_instance.midaz.address
}
